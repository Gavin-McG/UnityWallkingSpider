using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackColliderTest : MonoBehaviour
{
    public GameObject followObj;
    private TrackCollider tc;
    // Start is called before the first frame update
    void Start()
    {
        tc = GetComponent<TrackCollider>();
        
        tc.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tc.setCollider(followObj.GetComponent<Collider>());
        transform.position += tc.velocity * Time.deltaTime;
        transform.rotation = tc.angularVel * transform.rotation;
    }
}
