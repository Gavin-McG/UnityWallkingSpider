using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTarget : MonoBehaviour
{
    //Move the body towards a more ideal position using leg data collected from CastFromObjects

    public float heightOffset; //ideal height for body to position away from leg height
    public float attractStength; //strength that the body is attracted towards heightOffset
    public float rotateStrength; //Strength that the body rotates towards the average leg normal
    public float velMultiplier;
    public int ignoreSensors; //number of outlier sensors to ignore in rotation

    [HideInInspector] public float heightMultiplier = 1; //Used for jumping "animation"
    [HideInInspector] public bool isGrounded; //whther any of the legs are touching the floor
    [HideInInspector] public bool applyForce; //whether the rotation and forces should be applied (used in jumping process)
    
    //components
    private CastFromObject[] castObjects;
    private Rigidbody rb;
    private SpiderController sc;
    private MoveWithLegs ml;

    //stores initial vlaues for drag to switch between
    private float drag;
    private float angularDrag;

    //direction to rotate the body in
    private Vector3 rotDirection;
    private Vector3 averageNormal;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SpiderController>();
        ml = GetComponent<MoveWithLegs>();
        castObjects = transform.parent.transform.Find("Sensor Zones").GetComponentsInChildren<CastFromObject>();

        //save initial drag values
        drag = rb.drag;
        angularDrag = rb.angularDrag;

        //initial rot driection to avoid glitches in first frame
        rotDirection = transform.up;
        averageNormal = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        averageNormal = GetAverageNormal(2);

        //spider is grounded if any legs are touching a collider

        if (applyForce)
        {
            isGrounded = false;
            for (int i = 0; i < castObjects.Length; i++)
            {
                if (castObjects[i].isConnected)
                {
                    isGrounded = true;
                }
            }

            if (isGrounded)
            {
                rb.velocity = ml.averageVelocity + (rb.velocity-ml.averageVelocity)*velMultiplier;
                //update rotation direction w/ attempt to smooth movement
                rotDirection = 0.7f * Vector3.Cross(transform.up, averageNormal) + 0.3f * rotDirection;
                //Rotate the body towards the direction of the average normal
                rb.AddTorque(rotDirection * rotateStrength);
            }


            //Move the body towards the ideal height relative to the legs
            for (int i = 0; i < castObjects.Length; i++)
            {
                if (castObjects[i].isConnected)
                {
                    //body's signed distance from the plane defined by the leg point's normal offset by heightOffset
                    Vector3 forceDir = (heightOffset * heightMultiplier * averageNormal - Vector3.Project(transform.position - castObjects[i].castPoint, averageNormal));
                    //move body towards heightOffset away from the plane befined by leg normal
                    rb.AddForce(forceDir * attractStength);
                }

            }
        }
        
        //reduce drag and return gravity when the spider is no longer touching the ground
        if (isGrounded)
        {
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.useGravity = false;
            sc.canJump = true;
        }
        else
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
            rb.useGravity = true;
            sc.canJump = false;
        }

    }

    struct NormalMap
    {
        public int index;
        public float normalDiffernece;
    }
    private Vector3 GetAverageNormal(int outliers)
    {
        //Find the average nornal of all of the points the leg are on and whether its on the ground
        Vector3 averageNormal = Vector3.zero;

        //get total differences between fellow normals
        List<NormalMap> normalList = new List<NormalMap>();
        for (int i=0; i <castObjects.Length; i++)
        {
            NormalMap m = new NormalMap();
            m.index = i;
            m.normalDiffernece = 0;
            for (int j=0; j<castObjects.Length; j++)
            {
                if (i!=j)
                {
                    m.normalDiffernece += (castObjects[i].castNormal - castObjects[j].castNormal).magnitude;
                }
            }
            normalList.Add(m);
        }

        //sort normalList
        for (int i=1; i<normalList.Count; i++)
        {
            if (i==0)
            {
                i = 1;
            }
            if (normalList[i].normalDiffernece < normalList[i-1].normalDiffernece)
            {
                NormalMap temp = normalList[i];
                normalList[i] = normalList[i - 1];
                normalList[i - 1] = temp;
                i -= 2;
            }
        }


        for (int i = 0; i < normalList.Count - outliers; i++)
        {
            averageNormal += castObjects[normalList[i].index].castNormal;
        }
        return averageNormal.normalized;
    }
}
