using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithLegs : MonoBehaviour
{

    private StickToCollider[] stickToColliders;
    private HoldManager hm;
    private BodyTarget bt;

    // Start is called before the first frame update
    void Start()
    {
        hm = GetComponent<HoldManager>();
        bt = GetComponent<BodyTarget>();
        stickToColliders = transform.parent.Find("Leg Points").GetComponentsInChildren<StickToCollider>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (bt.isGrounded)
        {
            Vector3 averageVelocity = Vector3.zero;
            for (int i = 0; i < stickToColliders.Length; i++)
            {
                averageVelocity += stickToColliders[i].GetVelocity();
            }
            averageVelocity /= 8;
            
            transform.position += averageVelocity;
        }

    }
}
