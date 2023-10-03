using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegManager : MonoBehaviour
{
    //Update the variable values of all legs when given value is non-zero

    [SerializeField] float zoneRange; //distance from ideal position required for the leg to travel back to position
    [SerializeField] float stepDuration; //time it takes the leg to return to position
    [SerializeField] float stepHeight; //height that the leg ifts when returning

    [Space(10)]

    [SerializeField] float legLength; //Length of legs
    [SerializeField] float diameter; //width/diameter of legs
    [SerializeField] bool hasKnee; //whether to show kneeprefab

    [Space(10)]

    private float maxRange; //range from body that the leg can be placed (usually 2*legLength)

    //Compoents that variables affect
    private ReachZone[] reachZones;
    private ConnectLeg[] connectLegs;
    private CastFromObject[] castObjects;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        reachZones = GetComponentsInChildren<ReachZone>();
        connectLegs = GetComponentsInChildren<ConnectLeg>();
        castObjects = GetComponentsInChildren<CastFromObject>();

        //update variable every 2 seconds
        InvokeRepeating("updateValues", 0, 2);
    }

    private void updateValues()
    {
        maxRange = legLength * 2;

        //update values for reachzones
        for (int i = 0; i < reachZones.Length; i++)
        {
            if (zoneRange != 0)
            {
                reachZones[i].zoneRange = zoneRange;
            }
            if (stepDuration != 0)
            {
                reachZones[i].stepDuration = stepDuration;
            }
            if (stepHeight != 0)
            {
                reachZones[i].stepHeight = stepHeight;
            }
        }

        //update values for connectLegs
        for (int i = 0; i < connectLegs.Length; i++)
        {
            if (legLength != 0)
            {
                connectLegs[i].legLength = legLength;
            }
            if (diameter != 0)
            {
                connectLegs[i].diameter = diameter;
            }
            connectLegs[i].hasKnee = hasKnee;
        }

        //update values for CastFromObjects
        for (int i=0; i<castObjects.Length; i++)
        {
            if (maxRange != 0)
            {
                castObjects[i].maxRange = maxRange;
            }
        }
    }
}
