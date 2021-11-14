using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private float upForce;
    [SerializeField] private float rotationTorque;

    [SerializeField] private float maxAngularVelocity;
    [SerializeField] private float minAngularVelocity = 1.5f;


    [SerializeField] private float maxVelocityX= 5f;
    [SerializeField] private float maxVelocityY= 8f;


    private bool gameStarted = false;
    private bool isGrounded = true;
    private bool alive = true;
    private bool hasWon = false;

    private ChickenBuilding chickenBuilding;

    private CamScript camScript;
    private Camera cam;

    private Vector3 startCenterOfMass;

    public Transform partsTransform;

    [SerializeField] private GameObject pickupParticle;


    private Vector3 startScale;

    private bool justHitPropulsor = false;


    [SerializeField] private AudioClip[] jumpSounds = new AudioClip[3];
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        cam = Camera.main;
        startScale = transform.localScale;
        LeanTouch.OnFingerDown += OnFingerDown;
        rb = GetComponent<Rigidbody>();
        startCenterOfMass = rb.centerOfMass;

        rb.isKinematic = true;
        
        camScript = FindObjectOfType<CamScript>();
        chickenBuilding = GetComponent<ChickenBuilding>();

        camScript.Init(transform);
    }

    private void OnFingerDown(LeanFinger obj)
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    void Update()
    {
        if (!alive) return;

        if (!gameStarted) return;
        float angularVelocityZ = rb.angularVelocity.z;
        angularVelocityZ = Mathf.Clamp(angularVelocityZ, -maxAngularVelocity, -minAngularVelocity);
        rb.angularVelocity = new Vector3(0, 0, angularVelocityZ);

        float xVelocity = rb.velocity.x;
        float yVelocity = rb.velocity.y;
        xVelocity = Mathf.Clamp(xVelocity, -2, maxVelocityX);
        yVelocity = Mathf.Clamp(yVelocity, -50, maxVelocityY);
        rb.velocity = new Vector3(xVelocity, yVelocity, rb.velocity.z);
    }

    private void Jump()
    {

        bool firstJump = false;

        isGrounded = false;
        if (!gameStarted)
        {
            gameStarted = true;
            firstJump = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        Vector3 jumpDirection = transform.up;
        float jumpX = Mathf.Clamp(jumpDirection.x, -0.1f, 50);
        float jumpY = Mathf.Clamp(jumpDirection.y, 0.3f, 50);
        jumpDirection = new Vector3(jumpX, jumpY, jumpDirection.z);

        //ADD FORCE AND ROTATION
        rb.AddForce(jumpDirection * upForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.back * rotationTorque, ForceMode.Impulse);


        //PLAY RANDOM SOUND
        if (Random.Range(0f, 1f) < 0.5f) return;
        AudioClip randClip = jumpSounds[Random.Range(0, jumpSounds.Length)];
        source.clip = randClip;
        source.Play();
    }

    private void Spin()
    {
        rb.AddTorque(new Vector3(0, 1,-1)* rotationTorque, ForceMode.Impulse);
    }

    public void Lose()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        camScript.OnGameEnded(false);
        alive = false;
        chickenBuilding.OnLose();
        GameManager.instance.OnGameOver();
        
    }

    public void Win()
    {
        if (!alive) return;
        camScript.OnGameEnded(true, gameObject);
        hasWon = true;
        GameManager.instance.OnSuccess();
    }

    public void ResetCenterOfMass()
    {
        //Vector3 newCenter = rb.centerOfMass;
        //rb.centerOfMass = new Vector3(newCenter.x, newCenter.y, startCenterOfMass.z);

        //print(rb.centerOfMass);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasWon || !alive) return;

        //foreach (var c in collision.contacts)
        //{
        //    if (c.thisCollider.GetComponent<ChickenDeathCollision>())
        //    {
        //        Lose();
        //        return;
        //    }
        //}

        if (collision.collider.CompareTag("Blade"))
        {
            Lose();
            return;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Propulsor propulsor = other.GetComponent<Propulsor>();

        if (propulsor && !justHitPropulsor)
        {
            justHitPropulsor = true;
            rb.AddForce(propulsor.transform.up * upForce * 3, ForceMode.Impulse);
            DOVirtual.DelayedCall(0.5f, () => { justHitPropulsor = false;  });
        }

        BodyPart part = other.GetComponent<BodyPart>();
        if (part)
        {
            transform.DOKill();
            transform.localScale = startScale;
            transform.DOPunchScale(transform.localScale, 0.1f, 1).SetEase(Ease.Linear).OnComplete(() => 
            {
                transform.localScale = startScale;
            });            
            Instantiate(pickupParticle, transform.position, Quaternion.identity);


            cam.DOShakePosition(0.05f, new Vector3(0, 0.05f, 0));

            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                Color startColor = renderer.material.color;
                renderer.material.color = Color.white;
                DOVirtual.DelayedCall(0.15f, () => { renderer.material.color = startColor; });
            }

            chickenBuilding.OnBodyPickup(part);
        }
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
    }


}
