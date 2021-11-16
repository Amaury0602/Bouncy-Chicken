using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BodyPart : MonoBehaviour
{
    public PartType partType;
    private bool isPickedUp = false;

    private Collider pickupCollider;

    [SerializeField] private List<Collider> bodyColliders = new List<Collider>();

    public List<AttachPoint> attachPoints = new List<AttachPoint>();

    [SerializeField] private GameObject shineParticle;


    private Vector3 startScale;

    private Animator anim;

    private void Awake()
    {
        startScale = transform.localScale;
        pickupCollider = GetComponentInChildren<Collider>();
        if (bodyColliders.Count > 0)
        {
            foreach(var b in bodyColliders)
            {
                b.enabled = false;
            }
        }

        if (partType == PartType.LWing || partType == PartType.RWing || partType == PartType.Feet)
        {
            transform.localScale *= 1.3f;
        }

        anim = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        if (anim != null)
        {

            anim.SetTrigger("Play");
        }
    }

    public void OnPickup(Transform destination, PlayerController player)
    {
        isPickedUp = true;

        shineParticle.SetActive(false);

        transform.position = destination.position;
        transform.SetParent(player.partsTransform);
        transform.localEulerAngles = Vector3.zero;

        transform.localScale = Vector3.zero;
        transform.DOScale(startScale, 0.15f).SetEase(Ease.InQuint);

        pickupCollider.enabled = false;

        if (bodyColliders.Count > 0)
        {
            foreach (var b in bodyColliders)
            {
                b.enabled = true;
            }
        }
    }

    public void Dismember()
    {
        if (bodyColliders.Count > 0)
        {
            foreach(var b in bodyColliders)
            {
                b.gameObject.layer = 0;
            }
        }
        else
        {
            gameObject.layer = 0;
        }
        Rigidbody newRB = gameObject.AddComponent<Rigidbody>();
        newRB.isKinematic = false;
        newRB.useGravity = true;
        newRB.AddForce(new Vector3(Random.Range(-3,3), Random.Range(3, 5), 0), ForceMode.Impulse);
    }
}

public enum PartType 
{
    LWing, RWing, Body, Feet, Head
}

[System.Serializable]
public class AttachPoint
{
    public Transform point;
    public PartType type;
    public bool used = false;
}
