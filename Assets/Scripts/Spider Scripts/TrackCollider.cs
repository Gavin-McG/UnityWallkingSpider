using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCollider : MonoBehaviour
{
    public Collider mainCollider;
    private Rigidbody objRb;
    public Vector3 velocity;
    public Quaternion angularVel;

    private Vector3 colliderPosition;
    private Quaternion colliderRotation;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
        angularVel = Quaternion.identity;
        mainCollider = new Collider();
        objRb = new Rigidbody();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVelocity();
        UpdateCollliderData();
    }

    private void OnDisable()
    {
        velocity = Vector3.zero;
        angularVel = Quaternion.identity;
    }

    private void OnEnable()
    {
        UpdateCollliderData();
    }


    private void UpdateVelocity()
    {
        if (mainCollider != null)
        {
            angularVel = mainCollider.transform.rotation * Quaternion.Inverse(colliderRotation);
            Vector3 newPos = angularVel * (transform.position - colliderPosition) + mainCollider.transform.position;
            velocity = newPos - transform.position;
            velocity /= Time.deltaTime;
        }
    }

    private void UpdateCollliderData()
    {
        if (mainCollider != null)
        {
            colliderPosition = mainCollider.transform.position;
            colliderRotation = mainCollider.transform.rotation;
        }
    }

    public void setCollider(Collider newCollider)
    {
        if (mainCollider != newCollider)
        {
            mainCollider = newCollider;
            objRb = newCollider.transform.GetComponent<Rigidbody>();
            UpdateCollliderData();
            velocity = Vector3.zero;
            angularVel = Quaternion.identity;
        }
    }
}