using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public float speed = 19000;
    public float rotateSpeed = 4400;
    public bool canJump = false;

    [Space(10)]

    public float jumpPower = 6000;
    public float jumpAngle = 20;
    public float chargeDuration = 0.8f;

    protected Rigidbody rb; //player's rigidbbody
    protected BodyTarget bt; //player's BodyTarget
    protected HoldManager hm; //player's HoldManager
    protected MoveWithLegs ml; //player's MoveWithLegs

    public virtual void Start()
    {
        //get components
        rb = GetComponent<Rigidbody>();
        bt = GetComponent<BodyTarget>();
        hm = GetComponent<HoldManager>();
        ml = GetComponent<MoveWithLegs>();
    }
    public void JumpProtocol()
    {
        //disbale forces from legs temproarily
        bt.applyForce = false;
        bt.isGrounded = false;
        ml.enabled = false;
        CancelInvoke();
        Invoke("EnableBT", 0.3f);
    }

    //enable the physics from bodyTarget
    private void EnableBT()
    {
        bt.applyForce = true;
        ml.enabled = true;
    }
}
