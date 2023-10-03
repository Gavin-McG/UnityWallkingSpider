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

    private GameObject[] objects; //All potential objects to place object onto
    private List<Collider> colliders = new List<Collider>(); // colliders of platforms

    // Start is called before the first frame update
    void Start()
    {
        //get components
        rb = GetComponent<Rigidbody>();
        bt = GetComponent<BodyTarget>();
        hm = GetComponent<HoldManager>();

        //get colliders of all pickupable objects
        objects = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < objects.Length; i++)
        {
            colliders.Add(objects[i].GetComponent<Collider>());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //player can only move when attached to the ground
        if (bt.isGrounded)
        {
            //forward and back
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.forward * speed * Time.deltaTime);
                cancelJump();
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(transform.forward * -speed * Time.deltaTime);
                cancelJump();
            }

            //rotate clockwise and counterclockwise
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddTorque(transform.up * (-rotateSpeed * Time.deltaTime));
                cancelJump();
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddTorque(transform.up * (rotateSpeed * Time.deltaTime));
                cancelJump();
            }

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

            float minDist = pickupDist;
            int minIndex = -1;

            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].Raycast(ray, out hit, pickupDist) && hit.distance < minDist)
                {
                    minDist = hit.distance;
                    minIndex = i;
                }
            }

            //if an object was found then pickup that object
            if (minIndex != -1)
            {
                hm.UpdateObject(colliders[minIndex].gameObject);
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
                hm.dropObject(0);
            }
            
        }
    }

    //enable the physics from bodyTarget
    private void EnableBT()
    {
        bt.applyForce = true;
    }

    //stop charging process
    private void cancelJump()
    {
        chargingJump = false;
        bt.heightMultiplier = 1;
    }
}
