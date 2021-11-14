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

    private GameObject playerModel;


    [SerializeField] private ParticleSystem rewardGlow;



    public void Init(Transform player)
    {
        target = player;
        transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);
        actualSpeed = followSpeed;

        if (playerModel != null) 
        {
            playerModel.transform.DOKill();

            if (rewardGlow.isPlaying)
            {
                rewardGlow.gameObject.SetActive(false);
                rewardGlow.gameObject.SetActive(true);
            }
            Destroy(playerModel);
        }
        
            
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

    public void OnGameEnded(bool win, GameObject player = null)
    {
        float newSpeed = win ? actualSpeed / 2 : 0;

        DOTween.To(() => actualSpeed, x => actualSpeed = x, newSpeed, 0.5f).SetUpdate(true);


        if (player == null) return;
        DOVirtual.DelayedCall(1f, () =>
        {
            rewardGlow.Play();
            playerModel = Instantiate(player.transform.GetChild(0).gameObject, transform.position, Quaternion.identity, transform);

            playerModel.transform.localPosition = new Vector3(0, 0, 5);
            playerModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerModel.transform.localScale = Vector3.zero;
            playerModel.transform.DOScale(new Vector3(2.9013f, 2.9013f, 2.248217f), 0.5f).SetEase(Ease.Linear).SetUpdate(true);

            playerModel.transform.DOLocalRotate(new Vector3(0.0f, 180, 0), 1.0f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        });
        
    }
}
