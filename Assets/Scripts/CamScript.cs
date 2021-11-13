using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamScript : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float followSpeed;

    private float actualSpeed;


    public void Init(Transform player)
    {
        target = player;
        transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);
        actualSpeed = followSpeed;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 nextPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);


            float xPos = Mathf.Lerp(transform.position.x, nextPosition.x, actualSpeed);
            float yPos = Mathf.Lerp(transform.position.y, nextPosition.y, actualSpeed / 100);
            float zPos = Mathf.Lerp(transform.position.z, nextPosition.z, actualSpeed);
            transform.position = new Vector3(xPos, yPos, zPos);
        }
    }

    public void OnGameEnded(bool win)
    {
        float newSpeed = win ? actualSpeed / 2 : 0;

        DOTween.To(() => actualSpeed, x => actualSpeed = x, newSpeed, 0.5f).SetUpdate(true);

    }
}
