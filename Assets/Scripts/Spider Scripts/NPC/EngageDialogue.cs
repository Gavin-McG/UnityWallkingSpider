using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageDialogue : MonoBehaviour
{
    [SerializeField] GameObject conversation;
    private bool isTurning;
    private bool isQueueing;
    private SpiderController sc;
    private Rigidbody rb;
    private GameObject targetObject;

    private static DialogueManager dm;
    private Interaction firstInteraction;

    private ConversationResponse cr;

    private void Start()
    {
        sc = GetComponent<SpiderController>();
        rb = GetComponent<Rigidbody>();

        dm = GameObject.Find("Game Manager").GetComponent<DialogueManager>();
        BeginInteraction bi = conversation.GetComponent<BeginInteraction>();
        if (bi != null)
        {
            firstInteraction = bi.firstInteraction;
        }

        cr = GetComponent<ConversationResponse>();
        if (cr == null)
        {
            cr = gameObject.AddComponent<ConversationResponse>();
        }
    }

    private void FixedUpdate()
    {
        if (isTurning)
        {
            Vector3 targetProj = Vector3.Cross(transform.up, Vector3.Cross(targetObject.transform.position - transform.position, transform.up));
            float angle = Vector3.SignedAngle(transform.forward, targetProj, transform.up);

            if (angle > 10)
            {
                rb.AddTorque(transform.up * (sc.rotateSpeed * Time.deltaTime));
            }
            else if (angle < -10)
            {
                rb.AddTorque(transform.up * (-sc.rotateSpeed * Time.deltaTime));
            }
            else
            {
                Invoke("BeginDialogue", 0.6f);
                isTurning = false;
            }
        }
    }

    public void Engage(GameObject obj)
    {
        if (firstInteraction != null && !isQueueing)
        {
            isTurning = true;
            isQueueing = true;
            sc.enabled = false;
            targetObject = obj;
        }
    }
    private void BeginDialogue()
    {
        dm.StartDialogue(firstInteraction, cr);
        isQueueing = false;
        sc.enabled = true;
    }

    public void UpdateConversation(GameObject newConvo)
    {
        conversation = newConvo;
        firstInteraction = conversation.GetComponent<BeginInteraction>().firstInteraction;
    }
}
