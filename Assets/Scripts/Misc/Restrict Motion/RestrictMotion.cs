using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictMotion : MonoBehaviour
{
    // Update is called once per frame
    public virtual void Update()
    {
        RestrictTransform();
    }

    public virtual void RestrictTransform() { }
}
