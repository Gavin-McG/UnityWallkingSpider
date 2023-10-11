using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageDialogue : MonoBehaviour
{
    private bool isTurning;
    private ApproachTarget at;
    private Rigidbody rb;
    private GameObject targetObject;

    private void Start()
    {
        at = GetComponent<ApproachTarget>();
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        if (isTurning)
        {
            Vector3 targetProj = Vector3.Cross(transform.up, Vector3.Cross(targetObject.transform.position - transform.position, transform.up));
            float angle = Vector3.SignedAngle(transform.forward, targetProj, transform.up);

            if (angle > 10)
            {
                rb.AddTorque(transform.up * (at.rotateSpeed * Time.deltaTime));
            }
            else if (angle < -10)
            {
                rb.AddTorque(transform.up * (-at.rotateSpeed * Time.deltaTime));
            }
            else
            {
                Invoke("BeginDialogue", 0.6f);
            }
        }
    }

    public void Engage(GameObject obj)
    {
        isTurning = true;
        at.enabled = false;
        targetObject = obj;
    }
    private void BeginDialogue()
    {
        Time.timeScale = 0;
    }

    private void EndDialogue()
    {
        isTurning = false;
        at.enabled = true;
        Time.timeScale = 1;
    }
}
