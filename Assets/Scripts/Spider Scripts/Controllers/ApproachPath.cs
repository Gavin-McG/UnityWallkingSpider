using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachPath : SpiderController
{

    private float jumpTime = 0;
    private float heightReduction = 0.4f;
    private bool chargingJump = false;

    [System.Serializable] public struct Path
    {
        public Vector3 targetPos;
        public float range;
        public bool jump;
    }

    [Space(10)]
    public Path[] paths;
    public bool loop;
    private int pathsInd = 0;

    private Rigidbody rb; //player's rigidbbody
    private BodyTarget bt;
    private MoveWithLegs ml;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bt = GetComponent<BodyTarget>();
        ml = GetComponent<MoveWithLegs>();

        chargingJump = paths[pathsInd].jump;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bt.isGrounded)
        {
            Vector3 targetProj = Vector3.Cross(transform.up, Vector3.Cross(paths[pathsInd].targetPos - transform.position, transform.up));
            //Debug.Log(targetProj);
            float angle = Vector3.SignedAngle(transform.forward, targetProj, transform.up);
            //Debug.Log(angle);

            if (angle > 10)
            {
                rb.AddTorque(transform.up * (rotateSpeed * Time.deltaTime));
                cancelJump();
            }
            else if (angle < -10)
            {
                rb.AddTorque(transform.up * (-rotateSpeed * Time.deltaTime));
                cancelJump();
            }
            else if (chargingJump)
            {
                jumpTime += Time.deltaTime;
                jumpTime = Mathf.Min(jumpTime, chargeDuration);
                bt.heightMultiplier = 1 - (jumpTime / chargeDuration) * heightReduction;
                if (!canJump)
                {
                    cancelJump();
                }

                if (jumpTime >= chargeDuration)
                {
                    Jump();
                }
            }
            else if ((paths[pathsInd].targetPos - transform.position).magnitude > paths[pathsInd].range)
            {
                rb.AddForce(transform.forward * speed * Time.deltaTime);
            }
            else if (UpdatePathIndex())
            {
                chargingJump = paths[pathsInd].jump;
            }
        }
    }

    private bool UpdatePathIndex()
    {
        if (pathsInd >= paths.Length - 1)
        {
            if (loop)
            {
                pathsInd = 0;
                return true;
            }
            return false;
        }
        else
        {
            pathsInd++;
        }
        return true;
    }

    private void Jump()
    {
        //disbale forces from legs temproarily
        bt.applyForce = false;
        bt.isGrounded = false;
        ml.enabled = false;
        Invoke("EnableBT", 0.3f);

        //reset variable used in charging
        cancelJump();
        canJump = false;
        chargingJump = false;

        //force of jump
        rb.AddForce(jumpPower * (Quaternion.AngleAxis(jumpAngle, transform.right) * transform.up) * (0.3f + 0.7f * jumpTime / chargeDuration));
    }

    private void EnableBT()
    {
        bt.applyForce = true;
        ml.enabled = true;
    }

    //stop charging process
    private void cancelJump()
    {
        jumpTime = 0;
        bt.heightMultiplier = 1;
    }
}
