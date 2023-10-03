using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldManager : MonoBehaviour
{
    //Manage the operations to pick up and drop an object

    public GameObject obj;
    public ConnectLeg[] legs;

    public bool isHolding = false;
    public float holdRotation;
    public float speedLimit;

    private ReachZone[] reachZones;
    private FollowObject[] followObjects;
    private HoldPosition holdPosition;
    private Rigidbody objRb;

    // Start is called before the first frame update
    void Start()
    {
        //get the components of each legs 
        reachZones = new ReachZone[legs.Length];
        followObjects = new FollowObject[legs.Length];

        for (int i = 0; i < legs.Length; i++)
        {
            reachZones[i] = legs[i].target.GetComponent<ReachZone>();
            followObjects[i] = legs[i].target.GetComponent<FollowObject>();
        }

        holdPosition = GetComponent<HoldPosition>();
    }

    //drop object
    public void dropObject(float dropForce)
    {
        isHolding = false;

        //reset angle of legs
        for (int i=0; i< legs.Length; i++)
        {
            legs[i].upwardsRotation = 0;
        }

        //swap legs back to walking
        for (int i = 0; i < followObjects.Length; i++)
        {
            followObjects[i].enabled = false;
        }
        for (int i = 0; i < reachZones.Length; i++)
        {
            reachZones[i].enabled = true;
        }
        holdPosition.enabled = false;

        //reset drag
        objRb.drag = 0.0f;
        //set velocity to this object's plus throw velocity
        objRb.velocity = GetComponent<Rigidbody>().velocity + transform.rotation * new Vector3(0,1,1) * dropForce;
        //limit velocity to speed limit (prevents super long throws when jumping
        if (objRb.velocity.magnitude > speedLimit)
        {
            objRb.velocity = objRb.velocity.normalized * speedLimit;
        }

        //set collider of object back on
        obj.GetComponent<Collider>().enabled = true;
    }

    //pickup Object
    public void UpdateObject(GameObject newObj)
    {
        //set values from newly held object
        obj = newObj;
        objRb = obj.GetComponent<Rigidbody>();
        isHolding = true;

        //set legs to more natural holding position
        for (int i = 0; i< legs.Length; i++)
        {
            legs[i].upwardsRotation = legs[i].isRight ? -holdRotation : holdRotation;
        }

        //swap legs from walking to holding
        for (int i = 0; i < followObjects.Length; i++)
        {
            followObjects[i].obj = obj;
            followObjects[i].enabled = true;
        }
        for (int i = 0; i < reachZones.Length; i++)
        {
            reachZones[i].enabled = false;
        }

        //update holdPosition with new object
        holdPosition.UpdateObj(obj);
        holdPosition.enabled = true;

        //set drag of object for more natural-looking holding
        objRb.drag = 0.7f;

        //prevent object from hitting spider when jumping
        obj.GetComponent<Collider>().enabled = false;
    }
}
