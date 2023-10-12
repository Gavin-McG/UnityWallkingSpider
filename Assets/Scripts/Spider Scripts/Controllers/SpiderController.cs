using System.Collections;
using System.Collections.Generic;
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
}
