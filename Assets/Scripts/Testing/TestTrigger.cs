using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrigger : MonoBehaviour
{

    public GameObject triggerObject;
    private Interactible trigger;
    private LinearRotation lr;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        trigger = triggerObject.GetComponent<Interactible>();
        lr = GetComponent<LinearRotation>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.enabled = trigger.triggered;
        rb.constraints = trigger.triggered ? RigidbodyConstraints.FreezePosition : RigidbodyConstraints.FreezeAll;
    }
}
