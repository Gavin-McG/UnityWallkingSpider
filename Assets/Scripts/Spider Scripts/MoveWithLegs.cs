using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithLegs : MonoBehaviour
{
    public Vector3 averageVelocity;
    public float followForce;
    public float followThreshold;

    private TrackCollider[] trackColliders;
    private HoldManager hm;
    private BodyTarget bt;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        hm = GetComponent<HoldManager>();
        bt = GetComponent<BodyTarget>();
        rb = GetComponent<Rigidbody>();
        trackColliders = transform.parent.Find("Sensor Zones").GetComponentsInChildren<TrackCollider>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (bt.isGrounded)
        {
            averageVelocity = Vector3.zero;
            for (int i = 0; i < trackColliders.Length; i++)
            {
                averageVelocity += trackColliders[i].velocity;
            }
            averageVelocity /= 8;

            Vector3 offset = averageVelocity - rb.velocity;
            rb.AddForce(offset.normalized * Mathf.Min(offset.magnitude, followThreshold) * followForce);

            rb.MoveRotation(trackColliders[0].angularVel * rb.rotation);


            /*Quaternion newRotation = Quaternion.identity;
            for (int i = trackColliders.Length-1; i >= 0;i--)
            {
                newRotation *= Quaternion.Slerp(Quaternion.identity, trackColliders[i].angularVel, 1.0f / trackColliders.Length);
            }
            newRotation *= rb.rotation;
            rb.MoveRotation(newRotation);*/
        }
    }
}
