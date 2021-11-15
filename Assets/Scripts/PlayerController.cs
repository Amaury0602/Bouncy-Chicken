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

    private GameUI gameUI;


    //AUDIO
    [SerializeField] private AudioClip[] jumpSounds = new AudioClip[3];
    [SerializeField] private AudioClip winSound;
    private AudioSource source;


    //ARROW
    [SerializeField] private GameObject arrow;
    private MeshRenderer arrowMesh;
    private Color arrowStartColor;
    private bool canDisplayArrow = false;
    private Tween arrowTween;


    [SerializeField] private GameObject poofEffect;



    


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
        arrowMesh = arrow.transform.GetComponentInChildren<MeshRenderer>();
        arrowStartColor = arrowMesh.material.color;
        gameUI = FindObjectOfType<GameUI>();

        gameUI.ShowPanel(false);
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

    private bool detectGround = false;
    private void Jump()
    {
        if (PlayerPrefs.GetInt("Level") == 0)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                canDisplayArrow = true;
            });
            if (Time.timeScale < 1)
            {
                arrow.SetActive(false);
                print(arrowStartColor);
                arrowTween.Kill();
                arrowMesh.material.color = arrowStartColor;
                Time.timeScale = 1;
                gameUI.ShowPanel(false);
            }
        }


        isGrounded = false;

        if (!gameStarted)
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                detectGround = true;
            });
            gameStarted = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            gameUI.HideTapStartTuto();
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
        source.clip = winSound;
        source.Play();
        gameUI.ShowPanel(true);
        camScript.OnGameEnded(true, gameObject);
        hasWon = true;
        GameManager.instance.OnSuccess();
    }

    int arrowCount = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (hasWon || !alive || !gameStarted) return;


        if (collision.collider.CompareTag("Blade"))
        {
            Lose();
            return;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            if (PlayerPrefs.GetInt("Level") == 0 && canDisplayArrow && arrowCount < 2)
            {
                DisplayArrowTutorial();
            }
            if (detectGround) isGrounded = true;
        }
    }

    private void DisplayArrowTutorial()
    {
        arrowCount++;
        canDisplayArrow = false;
        Time.timeScale = 0.1f;
        gameUI.ShowPanel(true, true);
        arrow.SetActive(true);
        arrow.transform.DOKill();
        arrowMesh.material.color = arrowStartColor;
        arrowTween = arrowMesh.material.DOColor(Color.black, 0.25f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true).SetEase(Ease.Linear);
    }


    private Propulsor lastHitPropulsor;
    private void OnTriggerEnter(Collider other)
    {

        Propulsor propulsor = other.GetComponent<Propulsor>();

        if (propulsor && propulsor != lastHitPropulsor)
        {
            lastHitPropulsor = propulsor; 
            Instantiate(poofEffect, transform.position, transform.rotation);
            justHitPropulsor = true;
            rb.AddForce(propulsor.transform.up * upForce * 7, ForceMode.Impulse);
            DOVirtual.DelayedCall(0.5f, () => { justHitPropulsor = false;  });
        }

        BodyPart part = other.GetComponent<BodyPart>();
        if (part)
        {
            transform.DOKill();
            transform.localScale = startScale;
            transform.DOPunchScale(transform.localScale, 0.1f, 1).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() => 
            {
                transform.localScale = startScale;
            });            
            Instantiate(pickupParticle, transform.position, Quaternion.identity);


            cam.DOShakePosition(0.05f, new Vector3(0, 0.05f, 0)).SetUpdate(true);

            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                Color startColor = renderer.material.color;
                renderer.material.color = Color.white;
                DOVirtual.DelayedCall(0.15f, () => { renderer.material.color = startColor; }).SetUpdate(true);
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
