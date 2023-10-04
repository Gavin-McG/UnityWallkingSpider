using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTarget : MonoBehaviour
{
    //Move the body towards a more ideal position using leg data collected from CastFromObjects

    public float heightOffset; //ideal height for body to position away from leg height
    [HideInInspector] public float heightMultiplier = 1; //Used for jumping "animation"
    public float attractStength; //strength that the body is attracted towards heightOffset
    public float rotateStrength; //Strength that the body rotates towards the average leg normal
    public bool isGrounded; //whther any of the legs are touching the floor
    public float gravity; //force of gravity when no legs are touching the ground
    public float velMultiplier;
    public bool applyForce; //whether the rotation and forces should be applied (used in jumping process)
    
    //components
    private CastFromObject[] castObjects;
    private Rigidbody rb;
    private PlayerController pc;

    //stores initial vlaues for drag to switch between
    private float drag;
    private float angularDrag;

    //direction to rotate the body in
    private Vector3 rotDirection;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        castObjects = transform.parent.transform.Find("Sensor Zones").GetComponentsInChildren<CastFromObject>();

        //save initial drag values
        drag = rb.drag;
        angularDrag = rb.angularDrag;

        //initial rot driection to avoid glitches in first frame
        rotDirection = transform.up;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Find the average nornal of all of the points the leg are on and whether its on the ground
        Vector3 averageNormal = Vector3.zero;
        isGrounded = false;
        for (int i=0;i<castObjects.Length; i++)
        {
            averageNormal += castObjects[i].castNormal;
            if (castObjects[i].isConnected)
            {
                isGrounded = true;
            }
            
        }
        averageNormal = averageNormal.normalized;

        if (applyForce)
        {
            if (isGrounded)
            {
                rb.velocity *= velMultiplier;
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
            if (applyForce)
            {
                pc.canJump = true;
            }
        }
        else
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
            rb.AddForce(Vector3.up * -gravity);
            pc.canJump = false;
        }

    }

    
}
