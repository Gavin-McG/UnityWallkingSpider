using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    public float bounceForce;
    public float coolDown;

    private float timer;

    private void Update()
    {
        timer -=Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (timer < 0)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, transform.up);
            rb.AddForce(transform.up * bounceForce);
            rb.angularVelocity *= 0.2f;

            SpiderController sc = other.GetComponent<SpiderController>();
            if (sc != null)
            {
                sc.JumpProtocol();
            }

            timer = coolDown;

            transform.parent.GetComponent<Animator>().SetTrigger("Bounce");
        }
    }
}
