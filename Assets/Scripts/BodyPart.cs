using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BodyPart : MonoBehaviour
{
    public PartType partType;
    private bool isPickedUp = false;

    private Collider pickupCollider;

    [SerializeField] private Collider bodyCollider;

    public List<AttachPoint> attachPoints = new List<AttachPoint>();

    [SerializeField] private GameObject shineParticle;


    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
        pickupCollider = GetComponentInChildren<Collider>();
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
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
        transform.DOScale(startScale, 0.25f).SetEase(Ease.InQuint);

        pickupCollider.enabled = false;

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }
    }

    public void Dismember()
    {
        if (bodyCollider != null)
        {
            bodyCollider.gameObject.layer = 0;
        } else
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
