using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationResponse : MonoBehaviour
{
    public virtual void Respond(int code) 
    {
        Debug.Log("Dafault Convo Response");
    }
}
