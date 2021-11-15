using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Propulsor : MonoBehaviour
{
    private void Update()
    {
        transform.GetChild(0).Rotate(Vector3.up * 150 * Time.deltaTime);
    }

    public void OnActivation()
    {
        transform.GetChild(0).DOShakeScale(0.2f, transform.GetChild(0).localScale, 5);
    }
}
