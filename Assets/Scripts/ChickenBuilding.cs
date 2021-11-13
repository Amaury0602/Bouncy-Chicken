using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChickenBuilding : MonoBehaviour
{
    private PlayerController player;

    public List<BodyPart> actualParts = new List<BodyPart>();

    private BodyPart headPart;

    private bool hasBody = false;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        headPart = actualParts[0];
    }


    public void OnBodyPickup(BodyPart part)
    {
        if (actualParts.Contains(part)) return;
       
        BodyPart mainPart = headPart;

        foreach (var _part in actualParts)
        {
            if (_part.partType == PartType.Body)
            {
                mainPart = _part;
            }
        }


#if UNITY_EDITOR
        //print(mainPart.partType);

        //foreach (var item in mainPart.attachPoints)
        //{
        //    print(item.type);
        //}
#endif


        Transform destination = null;

        switch (part.partType)
        {
            case PartType.LWing:
                destination = mainPart.attachPoints.Single(attach => attach.type == PartType.LWing).point;
                break;
            case PartType.RWing:
                destination = mainPart.attachPoints.Single(attach => attach.type == PartType.RWing).point;
                break;
            case PartType.Body:
                hasBody = true;
                destination = mainPart.attachPoints.Single(attach => attach.type == PartType.Body).point;
                break;
            case PartType.Feet:
                destination = mainPart.attachPoints.Single(attach => attach.type == PartType.Feet).point;
                break;
            case PartType.Head:
                break;
            default:
                break;
        }

        actualParts.Add(part);

        part.OnPickup(destination, player);
        
        player.ResetCenterOfMass();
    }

    public void OnLose()
    {
        foreach (var part in actualParts)
        {
            part.Dismember();
        }
    }
}
