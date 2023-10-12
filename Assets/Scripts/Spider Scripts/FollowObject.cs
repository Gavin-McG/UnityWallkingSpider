using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    //Set the position of an object realtive to this transform with a given offset

    public GameObject obj;
    public GameObject center;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = obj.transform.position + center.transform.rotation * offset;
    }
}
