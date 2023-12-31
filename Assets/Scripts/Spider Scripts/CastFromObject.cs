using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CastFromObject : MonoBehaviour
{
    //Place the object relative to another object using raycasts
    //rays upwards of neutral chosen by lowest angle between normal angle and ray direction
    //rays below neutral chosen as last resort

    public GameObject centerObject; //object that the ray will be cast from
    public GameObject movementObject; //object with rigidbody attached
    public float maxRange; //max range of the cast

    [Space(10)]

    [SerializeField] string[] tags; //Tags of all objects to place object onto

    [Space(10)]

    [SerializeField] Vector3 castDir; //Direction of the neutral ray cast
    private Vector3 castDirRot; //rotated version of neutral raycast
    [SerializeField] float castShift; //angle of differece between each adjecent ray cast
    [SerializeField] int castCountUp; //number of casts to do upwards of the neutral ray
    [SerializeField] int castCountDown; //number of casts to do downwards of the neutral ray

    [Space(10)]

    [SerializeField] float velFactor; //amount to change ray direction using the velocity direction
    [SerializeField] float velThreashhold; //velocity required to change ray direction with VelFactor
    private Rigidbody rb;
    private MoveWithLegs ml;

    private Vector3 pastPos;
    private Vector3 pastFw;


    [HideInInspector] public Vector3 castNormal; //used by other scripts to affect body position
    [HideInInspector] public Vector3 castPoint; //holds the point the object will be cast onto
    [HideInInspector] public Collider castCollider; //The specific collider that was hit when casting
    [HideInInspector] public bool isConnected; //used by other scripts to determine when the body is touching the ground

    private TrackCollider tc;

    // Start is called before the first frame update
    void Start()
    {
        //If cast direction is a zero vector then the initial relation will be used as castDir
        if (castDir == Vector3.zero)
        {
            castDir = transform.position - centerObject.transform.position;
        }

        rb = movementObject.GetComponent<Rigidbody>();
        ml = movementObject.GetComponent<MoveWithLegs>();
        castCollider = new Collider();
        tc = GetComponent<TrackCollider>();
        
    }




    // Update is called once per frame
    void FixedUpdate()
    {
        //Create new directions of cast by rotating to the center object's rotation
        UpdateNeutralDirection();

        //set default values
        ResetRayCastValues();

        
        TestUpwardsRayCasts();

        if (!isConnected)
        {
            TestDownwardsRayCasts();
        }

        if (isConnected && castCollider.transform.CompareTag("Moving"))
        {
            tc.setCollider(castCollider);
            tc.enabled = true;
        }
        else
        {
            tc.enabled = false;
        }

        //draw cast normal in scene view
        Debug.DrawRay(castPoint, castNormal, Color.yellow);

        //update position and rotation to the position and normal of the position of the best rayCast
        transform.position = castPoint;
        transform.LookAt(castNormal+transform.position);
    }





    //return castDir rotated by the centerObjects rotation and with velocity factored in
    private Vector3 UpdateNeutralDirection()
    {
        //Create new directions of cast by rotating to the center object's rotation
        castDirRot = (centerObject.transform.rotation * castDir).normalized;
        if (velFactor == 0)
        {
            return castDirRot;
        }

        //legs anticipate step direction
        Vector3 velocity = (rb.position - pastPos) / Time.deltaTime - ml.averageVelocity;
        pastFw = Vector3.ProjectOnPlane(pastFw, rb.transform.up);
        float angle = Vector3.SignedAngle(pastFw, rb.transform.forward, rb.transform.up) / Time.deltaTime;
        pastPos = rb.position;
        pastFw = rb.transform.forward;
        castDirRot = Quaternion.AngleAxis(angle * velFactor * 6, rb.transform.up) * castDirRot;
        if (velocity.magnitude > velThreashhold)
        {
            castDirRot += Vector3.ProjectOnPlane(velocity*velFactor,centerObject.transform.up);
            //Debug.Log(angle);
        }
        

        return castDirRot;
    }


    private void ResetRayCastValues()
    {
        castNormal = Vector3.Cross(Vector3.Cross(centerObject.transform.up, castDirRot), centerObject.transform.up); //default normaal is outward to help rotate body around ledges
        castPoint = centerObject.transform.position + maxRange * castDirRot * 0.5f; //default position is straight outward from the middle castDir
        isConnected = false;
    }


    private void TestUpwardsRayCasts()
    {
        RaycastHit hit;

        //lowest angle for any upwards array
        float bestNormalAngle = 90;

        //find the ray upwards of neutral with the best normal angle
        for (int i = castCountUp; i >= 0; i--)
        {
            //direction of the ray
            Vector3 castDirF = Quaternion.AngleAxis(i * -castShift, Vector3.Cross(centerObject.transform.up, castDirRot)) * castDirRot;
            Ray ray = new Ray(centerObject.transform.position, castDirF);

            //find the collider with the closest distance to the ray
            //update values if theres a shorts collider in direction of this ray
            if (Physics.Raycast(ray, out hit, maxRange) && IsInTags(hit.collider.tag) && Vector3.Angle(-hit.normal, castDirF) < bestNormalAngle)
            {
                //update values for this ray
                bestNormalAngle = Vector3.Angle(-hit.normal, castDirF);
                isConnected = true;
                castNormal = hit.normal;
                castPoint = hit.point;
                castCollider = hit.collider;
            }
        }
    }



    private void TestDownwardsRayCasts()
    {
        RaycastHit hit;

        //find the firsst ray downwards of position if one hasnt been found
        for (int i = 0; i < castCountDown; i++)
        {
            //stop searching when a platform has been found
            

            //direction of the ray
            Vector3 castDirF = Quaternion.AngleAxis(i * castShift, Vector3.Cross(centerObject.transform.up, castDirRot)) * castDirRot;
            Ray ray = new Ray(centerObject.transform.position, castDirF);


            //find the collider with the lowest distance to the ray
            if (Physics.Raycast(ray, out hit, maxRange) && IsInTags(hit.collider.tag))
            {
                //update values if an intersection's been found
                isConnected = true;
                castNormal = hit.normal;
                castPoint = hit.point;
                castCollider = hit.collider;
                return;
            }
        }
    }

    private bool IsInTags(string tag)
    {
        for (int i = 0; i<tags.Length; i++) 
        { 
            if (string.Compare(tag, tags[i])==0)
            {
                return true;
            }
        }
        return false;
    }
}
