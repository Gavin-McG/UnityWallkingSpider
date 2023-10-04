using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearRotation : MonoBehaviour
{

    public float frequency;
    public Vector3 axis;

    private Quaternion originalRot;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        originalRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = axis.normalized * 2 * Mathf.PI * frequency;
    }
}
