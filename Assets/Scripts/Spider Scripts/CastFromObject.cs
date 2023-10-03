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


    private GameObject[] platforms; //All potential objects to place object onto
    private List<Collider> colliders = new List<Collider>(); // colliders of platforms

    [HideInInspector] public Vector3 castNormal; //used by other scripts to affect body position
    [HideInInspector] public Vector3 castPoint; //holds the point the object will be cast onto
    [HideInInspector] public Collider castCollider; //The specific collider that was hit when casting
    [HideInInspector] public bool isConnected; //used by other scripts to determine when the body is touching the ground
    

    // Start is called before the first frame update
    void Start()
    {
        //Get all colliders from objects tagged with any of the designated tags
        for (int i=0;i<tags.Length; i++)
        {
            platforms = GameObject.FindGameObjectsWithTag(tags[i]);
            for (int j = 0; j < platforms.Length; j++)
            {
                colliders.Add(platforms[j].GetComponent<Collider>());
            }
        }

        //If cast direction is a zero vector then the initial relation will be used as castDir
        if (castDir == Vector3.zero)
        {
            castDir = transform.position - centerObject.transform.position;
        }

        rb = movementObject.GetComponent<Rigidbody>();
        castCollider = new Collider();
    }





    // Update is called once per frame
    void Update()
    {
        //Create new directions of cast by rotating to the center object's rotation
        UpdateNeutralDirection();

        //set default values
        ResetRayCastValues();

        if (colliders.Count > 0)
        {
            TestUpwardsRayCasts();

            TestDownwardsRayCasts();

            //draw cast normal in scene view
            Debug.DrawRay(castPoint, castNormal, Color.yellow);

            //update position and rotation to the position and normal of the position of the best rayCast
            transform.position = castPoint;
            transform.LookAt(castNormal+transform.position);
        }
    }





    //return castDir rotated by the centerObjects rotation and with velocity factored in
    private Vector3 UpdateNeutralDirection()
    {
        //Create new directions of cast by rotating to the center object's rotation
        castDirRot = (centerObject.transform.rotation * castDir).normalized;
        //legs anticipate step direction
        if (rb.velocity.magnitude > velThreashhold)
        {
            castDirRot += Vector3.Cross(movementObject.transform.up, Vector3.Cross(rb.velocity, movementObject.transform.up)).normalized * velFactor;
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

            //default values for this ray
            float rayAngle = 90;
            Vector3 rayNormal = Vector3.zero;
            Vector3 rayPoint = Vector3.zero;
            float minDist = maxRange;
            bool didCollide = false;
            Collider rayCollider = new Collider();

            //find the collider with the closest distance to the ray
            for (int j = 0; j < colliders.Count; j++)
            {
                //update values if theres a shorts collider in direction of this ray
                if (colliders[j].Raycast(ray, out hit, maxRange) && hit.distance < minDist)
                {
                    //update values for this ray
                    rayAngle = Vector3.Angle(-hit.normal, castDirF);
                    rayNormal = hit.normal;
                    rayPoint = hit.point;
                    minDist = hit.distance;
                    didCollide = true;
                    rayCollider = colliders[j];
                }
            }

            //update values if this ray is a better spot to place leg
            if (didCollide && rayAngle < bestNormalAngle)
            {
                isConnected = true;
                bestNormalAngle = rayAngle;
                castNormal = rayNormal;
                castPoint = rayPoint;
                castCollider = rayCollider;
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
            if (isConnected)
            {
                break;
            }

            //direction of the ray
            Vector3 castDirF = Quaternion.AngleAxis(i * castShift, Vector3.Cross(centerObject.transform.up, castDirRot)) * castDirRot;
            Ray ray = new Ray(centerObject.transform.position, castDirF);

            //default value
            float minDist = maxRange;

            //find the collider with the lowest distance to the ray
            for (int j = 0; j < colliders.Count; j++)
            {
                if (colliders[j].Raycast(ray, out hit, maxRange) && hit.distance < minDist)
                {
                    //update values if an intersection's been found
                    castNormal = hit.normal;
                    castPoint = hit.point;
                    minDist = hit.distance;
                    isConnected = true;
                    castCollider = colliders[j];
                }
            }
        }
    }
}
