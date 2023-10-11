using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Interaction : MonoBehaviour
{
    public string interactionName;

    [TextArea(5, 20)]
    public string[] mainDialogue;
    [System.Serializable] public struct Response
    {
        public string prompt;
        public Interaction reply;
    }
    public Response[] responses;
}
