using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResponse : ConversationResponse
{
    public GameObject blockPrefab;
    public override void Respond(int code)
    {
        if (code == 2)
        {
            Instantiate(blockPrefab, transform.position + transform.forward * 2, transform.rotation);
        }
    }
}
