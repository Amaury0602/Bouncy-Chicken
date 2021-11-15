using UnityEngine;
using DG.Tweening;

public class FinishLine : MonoBehaviour
{

    private bool lineCrossed = false;

    [SerializeField] private ParticleSystem[] confettis = new ParticleSystem[2];


    public void Init()
    {
        foreach (var particle in confettis)
        {
            particle.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (lineCrossed) return;
        PlayerController player = other.transform.GetComponentInParent<PlayerController>();

        if (player)
        {
            lineCrossed = true;
            player.Win();


            foreach (var particle in confettis)
            {
                particle.Play();
            }
        }
    }
}
