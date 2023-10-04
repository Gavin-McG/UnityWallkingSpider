using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmonicMovement : MonoBehaviour
{
    public Vector3 offset;
    public float frequency;

    private Vector3 originalPos;

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        rb = GetComponent<Rigidbody>();
        Physics.autoSyncTransforms = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = Mathf.Sin(Time.timeSinceLevelLoad * frequency * 2 * Mathf.PI) * offset + originalPos;
        transform.position = (newPos);
    }
}
