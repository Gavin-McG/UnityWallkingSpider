using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictLever : RestrictMotion
{
    [HideInInspector] public float leverAngle = 0;

    public GameObject leverBase;
    public float leverLength;
    public float angleMargin;

    [Space(10)]

    public float bistableForce;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.autoSyncTransforms = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        RestrictTransform(); //restrict motion every frame
    }

    private void FixedUpdate()
    {
        rb.AddForce(leverBase.transform.up * -bistableForce); //Lever appoaches either end
    }

    public override void RestrictTransform() {

        Vector3 offset = transform.position - leverBase.transform.position; //offset between lever knob and lever base
        offset = Vector3.ProjectOnPlane(offset, leverBase.transform.right); //set offset onto the plane the lever should lie on
        offset = offset.normalized * leverLength; //set offset to a set length
        float angle = Mathf.Max(0, Vector3.Angle(offset, leverBase.transform.up) - angleMargin); //angle from neutral position
        leverAngle = Vector3.SignedAngle(offset, leverBase.transform.up, leverBase.transform.right); //angle from neutral position
        offset = Vector3.RotateTowards(offset, leverBase.transform.up, angle*Mathf.Deg2Rad,0); //set offset within the range of motion of the lever

        transform.position = leverBase.transform.position + offset; //set position with offset from leverBase
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(offset, leverBase.transform.up), offset); //orient handle to leverBase

        rb.velocity = Vector3.Project(rb.velocity, Vector3.Cross(offset,leverBase.transform.right)); //project velocity onto vector tangent to circle
        if (angle > 0)
        {
            //stop instantly is lever hits at either end
            rb.velocity = Vector3.zero;
        }
    }
}
