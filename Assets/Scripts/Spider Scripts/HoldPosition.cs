using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPosition : MonoBehaviour
{
    //Hold an object in place relative to the spider's body

    //object to be held
    public GameObject obj;

    public Vector3 goalOffset;//ideal offset from body
    public float limitDistance;//maximum distance that the object can be from the offset
    public float maxDistance;
    public float repelStrength;//strength of attraction of the object towards the ideal offset
    public float effectiveDrag;//multiplier of object's velocity to prevent unnatural looking swinging

    //rigidbodies
    private Rigidbody rb;
    private Rigidbody objRb;
    private RestrictMotion objRm;

    //hold the direction towards the ideal offset
    private Vector3 angleDirection = Vector3.zero; 

    // Start is called before the first frame update
    void Start()
    {
        //get rigidbodies
        rb = GetComponent<Rigidbody>();
        UpdateObjectComp();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj!=null)
        {
            //update the direction towards the ideal offset
            Vector3 goalOffsetRot = transform.rotation * goalOffset;
            Vector3 currentOffset = obj.transform.position - transform.position;
            angleDirection = goalOffsetRot - currentOffset;

            //limit distace from this transform
            if (angleDirection.magnitude > limitDistance)
            {
                angleDirection = angleDirection.normalized * limitDistance;
                obj.transform.position = transform.position + goalOffsetRot - angleDirection;
            }

            //update object's rotation to this transform's rotation
            obj.transform.rotation = transform.rotation;

            //run operation for if a held object has a restricted range of motion
            objRm.RestrictTransform();

            //Keep player in range of held object
            currentOffset = obj.transform.position - transform.position;
            angleDirection = currentOffset - goalOffsetRot;
            if (angleDirection.magnitude > maxDistance)
            {
                rb.MovePosition(transform.position + angleDirection.normalized * (angleDirection.magnitude - maxDistance));
                rb.velocity = Vector3.Project(rb.velocity, currentOffset);
            }
        }
    }

    //atract the object with the calculated angleDirection
    private void FixedUpdate()
    {
        objRb.AddForce(angleDirection * repelStrength);
        objRb.velocity *= effectiveDrag;
    }

    //get rigidbody and restrictMotion from a newly picked up object
    private void UpdateObjectComp()
    {
        objRb = obj.GetComponent<Rigidbody>();
        objRm = obj.GetComponent <RestrictMotion>();
        if (objRm == null) {
            objRm = obj.AddComponent<RestrictMotion>();
        }
    }

    //swap with a new object
    public void UpdateObj(GameObject newObj)
    {
        obj = newObj;
        UpdateObjectComp();
    }
}
