using UnityEngine;
using DG.Tweening;

public class FinishLine : MonoBehaviour
{

    private bool lineCrossed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (lineCrossed) return;
        PlayerController player = other.transform.GetComponentInParent<PlayerController>();

        if (player)
        {
            lineCrossed = true;
            player.Win();
        }
    }
}
