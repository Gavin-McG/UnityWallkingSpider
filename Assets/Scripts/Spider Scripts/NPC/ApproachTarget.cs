using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ApproachTarget : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;

    public Vector3 targetPos;

    [Space(10)]

    public bool wander;
    public float wanderDist;

    private Rigidbody rb; //player's rigidbbody
    private BodyTarget bt;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bt = GetComponent<BodyTarget>();

        if (wander)
        {
            InvokeRepeating("WanderTarget", Random.Range(0,4), 4.0f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bt.isGrounded)
        {
            Vector3 targetProj = Vector3.Cross(transform.up, Vector3.Cross(targetPos - transform.position, transform.up));
            //Debug.Log(targetProj);
            float angle = Vector3.SignedAngle(transform.forward, targetProj, transform.up);
            //Debug.Log(angle);

            if (angle > 10)
            {
                rb.AddTorque(transform.up * (rotateSpeed * Time.deltaTime));
            }
            else if (angle < -10)
            {
                rb.AddTorque(transform.up * (-rotateSpeed * Time.deltaTime));
            }
            else if ((targetPos - transform.position).magnitude > 4)
            {
                rb.AddForce(transform.forward * speed * Time.deltaTime);
            }
        }
    }

    private void WanderTarget()
    {
        LayerMask mask = LayerMask.GetMask("Ground", "Moving");
        RaycastHit hit = new RaycastHit();

        Vector3 rayDirection = Random.Range(0.1f, 1.0f)*transform.forward-transform.up;
        rayDirection = Quaternion.AngleAxis(Random.Range(0,360),transform.up) * rayDirection;

        Ray ray = new Ray(transform.position + transform.up * wanderDist, rayDirection);

        if (Physics.Raycast(ray, out hit, wanderDist*2, mask))
        {
            targetPos = hit.point;
        }

    }
}
