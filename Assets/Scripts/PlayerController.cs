using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private float jumpForceForward;
    [SerializeField] private float jumpForceUp;
    [SerializeField] private float upForce;
    [SerializeField] private float rotationTorque;


    private int jumpCount = 2;


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

    [SerializeField] private List<Material> materials = new List<Material>();
    
    // for tweening;


    [SerializeField] private List<Transform> bodyParts = new List<Transform>();

    void Start()
    {
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
        if (isGrounded && jumpCount > 0)
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

        jumpCount--;

        isGrounded = false;
        if (!gameStarted)
        {
            gameStarted = true;
            firstJump = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        //OLD WAY
        //rb.AddForce(new Vector3(jumpForceForward * transform.up.y, jumpForceUp, 0), ForceMode.Impulse);
        rb.AddForce(transform.up * upForce, ForceMode.Impulse);


        if (transform.up.y > 0.8f && !firstJump)
        {
            //Spin();
            //return;
        }

        /*if (jumpCount == 0) */
        rb.AddTorque(Vector3.back * rotationTorque, ForceMode.Impulse);
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
        camScript.OnGameEnded(true);
        hasWon = true;
        GameManager.instance.OnSuccess();
    }

    public void ResetCenterOfMass()
    {
        //rb.centerOfMass = startCenterOfMass;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasWon || !alive) return;

        foreach (var c in collision.contacts)
        {
            if (c.thisCollider.GetComponent<ChickenDeathCollision>())
            {
                Lose();
                return;
            }
        }

        if (collision.collider.CompareTag("Blade"))
        {
            Lose();
            return;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
