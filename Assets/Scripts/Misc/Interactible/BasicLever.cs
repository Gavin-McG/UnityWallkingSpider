using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLever : Interactible
{
    private RestrictLever restrictLever;

    // Start is called before the first frame update
    void Start()
    {
        restrictLever = GetComponent<RestrictLever>();
    }

    // Update is called once per frame
    void Update()
    {
        triggered = restrictLever.leverAngle > restrictLever.angleMargin - 20;
    }
}
