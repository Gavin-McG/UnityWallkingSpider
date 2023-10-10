using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Control the body using the Player's input

    public float speed;
    public float rotateSpeed;

    [Space(10)]

    public bool canJump;
    public float jumpPower;
    public float jumpAngle;
    public float chargeDuration;

    [Space(10)]

    public float pickupDist;
    public float dropForce;

    private float jumpTime = 0;
    private float heightReduction = 0.4f;
    private bool chargingJump = false;

    private Rigidbody rb; //player's rigidbbody
    private BodyTarget bt; //player's BodyTarget
    private HoldManager hm; //player's HoldManager
    private MoveWithLegs ml; //player's MoveWithLegs

    private LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody>();
        bt = GetComponent<BodyTarget>();
        hm = GetComponent<HoldManager>();
        ml = GetComponent<MoveWithLegs>();

        mask = LayerMask.GetMask("Pickup");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //player can only move when attached to the ground
        if (bt.isGrounded)
        {
            Vector3 moveDirection = Vector3.zero;
            //forward and back
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += transform.forward;
                cancelJump();
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection += -transform.forward;
                cancelJump();
            }

            //rotate clockwise and counterclockwise
            if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveDirection += -transform.right;
                    cancelJump();
                }
                else
                {
                    rb.AddTorque(transform.up * (-rotateSpeed * Time.deltaTime));
                    //cancelJump();
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveDirection += transform.right;
                    cancelJump();
                }
                else
                {
                    rb.AddTorque(transform.up * (rotateSpeed * Time.deltaTime));
                    //cancelJump();
                }
                //cancelJump();
            }

            moveDirection = moveDirection.normalized;
            rb.AddForce(moveDirection * speed * Time.deltaTime);

            //increment jump charge
            if (chargingJump)
            {
                jumpTime += Time.deltaTime;
                jumpTime = Mathf.Min(jumpTime, chargeDuration);
                bt.heightMultiplier = 1 - (jumpTime / chargeDuration) * heightReduction;
                if (!canJump)
                {
                    cancelJump();
                }
            }
        }
    }

    private void Update()
    {
        //start jumping process
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            jumpTime = 0;
            chargingJump = true;
            bt.heightMultiplier = 1;
        }

        //start jump
        if (Input.GetKeyUp(KeyCode.Space) && chargingJump)
        {
            //disbale forces from legs temproarily
            bt.applyForce = false;
            bt.isGrounded = false;
            ml.enabled = false;
            Invoke("EnableBT", 0.3f);

            //reset variable used in charging
            cancelJump();
            canJump = false;

            //force of jump
            rb.AddForce(jumpPower * (Quaternion.AngleAxis(jumpAngle, transform.right) * transform.up) * (0.3f + 0.7f * jumpTime / chargeDuration));
        }


        //pickup object
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F)) && !hm.isHolding)
        {
            //find the closest pickupable object directly in front of the spider
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupDist, mask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                hm.UpdateObject(hit.collider.gameObject);
            }
        }
        //drop object
        else if (Input.GetKeyDown(KeyCode.E) && hm.isHolding)
        {
            hm.dropObject(0);
        }
        //throw object
        else if (Input.GetKeyDown(KeyCode.F) && hm.isHolding)
        {
            if (bt.isGrounded)
            {
                hm.dropObject(dropForce);
            }
            else
            {
                hm.dropObject(dropForce*0.4f);
            }
            
        }
    }

    //enable the physics from bodyTarget
    private void EnableBT()
    {
        bt.applyForce = true;
        ml.enabled = true;
    }

    //stop charging process
    private void cancelJump()
    {
        chargingJump = false;
        bt.heightMultiplier = 1;
    }
}
