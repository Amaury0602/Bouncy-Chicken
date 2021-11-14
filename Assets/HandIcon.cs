using UnityEngine;
using DG.Tweening;

public class HandIcon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(transform.localScale * 1.3f, 0.5f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
