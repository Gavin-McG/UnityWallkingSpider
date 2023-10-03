using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReachZone : MonoBehaviour
{
    //Move an object to taget a given object in a way that resembles walking

    [SerializeField] GameObject zone; //object to target
    private CastFromObject zoneCast;

    [SerializeField] GameObject centerObject;

    public float zoneRange; //distance from the zone requires to take another "step"

    //variables for steps
    public float stepDuration; //time that a step takes
    private float stepTime = 0;//tracks time for active step
    public float stepHeight; //height of the step

    //variables for falling
    public float fallDuration; //time it takes to reach default position when falling
    private float fallTime = 0; //time of current fall
    private Vector3 defaultOffset; //initial offset of zone and body

    public bool isStepping = false; //whether a step is being take
    [HideInInspector] public bool canStep = true; //tracks whther a step can be taken

    //used to track path of step
    private Vector3 start;
    private Vector3 mid;
    private Vector3 end;

    private Rigidbody rb;
    private BodyTarget bt;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = zone.transform.position;

        zoneCast = zone.GetComponent<CastFromObject>();

        rb = centerObject.GetComponent<Rigidbody>();
        bt = centerObject.GetComponent<BodyTarget>();

        //offset to approach in free fall
        defaultOffset = zone.transform.position - centerObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //if the leg is far enough away from the zone then take a step
        if (!isStepping && canStep && (zone.transform.position - transform.position).magnitude > zoneRange)
        {
            //Set up values needed for stepping process
            canStep = false;
            stepTime = 0;

            //randomize step start time for more realistic walk cycle
            Invoke("beginStep", Random.Range(0, stepDuration / 2));
        }


        //approach default position while jumping/falling
        if (!zoneCast.isConnected)
        {
            //stop current step
            isStepping = false;
            canStep = true;

            //map between current and default position based on fall time
            transform.position = (1 - fallTime) * transform.position + fallTime * (centerObject.transform.position + centerObject.transform.rotation * defaultOffset);

            //fall time becomes 1 once fallDuration time has passed
            fallTime = Mathf.Min(fallTime + Time.deltaTime / fallDuration, 1);
        }
        else if (isStepping)
        {
            //reset fallTime
            fallTime = 0;
            
            //update time
            stepTime += Time.deltaTime / stepDuration;

            if (stepTime >= 1)
            {
                FinishStep();
            }
            else
            {
                //use Quadratic Bezier curve to calculate new leg position
                transform.position = getBezierPoint(stepTime, start, mid, end);
            }
        }
        else
        {
            //reset fallTime
            fallTime = 0;
        }
    }

    private void beginStep()
    {
        //set pooints for bezier path
        start = transform.position;
        end = zone.transform.position;
        mid = (start + end) / 2 + stepHeight * zone.transform.forward;
        isStepping = true;
    }

    //quadraric bezier function
    private Vector3 getBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Pow(t, 2) * (p1 - 2 * p2 + p3) + 2 * t * (p2 - p1) + p1;
    }

    private void FinishStep()
    {
        isStepping = false;
        canStep = true;
        transform.position = end;
    }
}
