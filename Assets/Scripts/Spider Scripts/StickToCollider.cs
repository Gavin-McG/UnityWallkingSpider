using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToCollider : MonoBehaviour
{
    [HideInInspector] public Collider mainCollider;
    [HideInInspector] public Vector3 position;
    private Vector3 velocity;

    private Vector3 colliderPosition;
    private Quaternion colliderRotation;

    private bool isSet = false;

    // Start is called before the first frame update
    void Start()
    {
        position = Vector3.zero;
        velocity = Vector3.zero;
        mainCollider = new Collider();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSet)
        {
            FollowCollider();
            UpdateCollliderData();
        }
    }


    private void FollowCollider()
    {
        //Quaternion rotDifference = stepCollider.transform.rotation * Quaternion.Inverse(previousRotation);
        //transform.position = rotDifference*(transform.position - previousPosition) + stepCollider.transform.position;
        velocity = mainCollider.transform.position - colliderPosition;
        position += velocity;
    }

    private void UpdateCollliderData()
    {
        colliderPosition = mainCollider.transform.position;
        colliderRotation = mainCollider.transform.rotation;
    }

    public void setCollider(Collider newCollider)
    {
        mainCollider = newCollider;
        UpdateCollliderData();
        velocity = Vector3.zero;
        isSet = true;
    }

    public void setPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

    public Vector3 GetVelocity()
    {
        if (isSet)
        {
            return velocity;
        }
        return Vector3.zero;
    }
}