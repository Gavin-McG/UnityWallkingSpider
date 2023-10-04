using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : MonoBehaviour
{
    public Material offMeterial;
    public Material onMeterial;

    public string[] tags;

    public int triggerCount = 0;
    public bool triggered = false;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        TestEnter(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
       TestExit(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TestEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TestExit(other);
    }

    private bool TestTag(Collider collider)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collider.transform.CompareTag(tags[i]))
            {
                return true;
            }
        }
        return false;
    }

    private void TestEnter(Collider collider)
    {
        if (TestTag(collider))
        {
            triggered = true;
            triggerCount++;
            meshRenderer.material = onMeterial;
        }
    }

    private void TestExit(Collider collider)
    {
        if (TestTag(collider))
        {
            triggerCount--;
            if (triggerCount == 0)
            {
                meshRenderer.material = offMeterial;
            }
        }
    }
}
