using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public bool isDialogueActive;

    public GameObject mainBox;
    public GameObject dialogue;
    public GameObject prompts;
    public GameObject[] replys;

    private Interaction currentInteraction;
    private ConversationResponse cr;

    private int dialogueStage = 0;
    private string currentText = "";
    private float charDelay = 0.01f;
    private float textTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (currentText.Length < currentInteraction.mainDialogue[dialogueStage].Length)
                {
                    currentText = currentInteraction.mainDialogue[dialogueStage];
                }
                else if (dialogueStage < currentInteraction.mainDialogue.Length-1)
                {
                    currentText = "";
                    dialogueStage++;
                }
                else 
                {
                    SetReplys();
                }
            }

            if (currentText.Length < currentInteraction.mainDialogue[dialogueStage].Length)
            {
                textTime += Time.fixedDeltaTime;
            }
            while (currentText.Length < currentInteraction.mainDialogue[dialogueStage].Length && charDelay < textTime)
            {
                currentText += currentInteraction.mainDialogue[dialogueStage][currentText.Length];
                textTime -= charDelay;
            }

            dialogue.transform.Find("Dialogue Text").GetComponent<TMP_Text>().text = currentText;
        }

    }

    public void StartDialogue(Interaction interaction, ConversationResponse conversationResponse)
    {
        ActivateDialogue(interaction);
        cr = conversationResponse;
    }

    public void choosePath(int opt)
    {
        Debug.Assert(opt < currentInteraction.responses.Length);
        ActivateDialogue(currentInteraction.responses[opt].reply);
    }

    private void ActivateDialogue(Interaction newInteration)
    {
        Debug.Assert(newInteration != null);

        currentInteraction = newInteration;

        Time.timeScale = 0.0f;
        textTime = 0.0f;
        mainBox.SetActive(true);
        dialogue.SetActive(true);
        prompts.SetActive(false);

        dialogueStage = 0;
        isDialogueActive = true;
        currentText = "";
    }

    public void endDialogue()
    {
        Time.timeScale = 1.0f;
        mainBox.SetActive(false);
        isDialogueActive = false;
    }

    private void SetReplys()
    {
        if (currentInteraction.responses.Length == 0)
        {
            endDialogue();
            cr.Respond(currentInteraction.returnCode);
            return;
        }

        dialogue.SetActive(false);
        prompts.SetActive(true);

        Debug.Assert(currentInteraction.responses.Length <= replys.Length);

        for (int i=0; i<currentInteraction.responses.Length; i++)
        {
            replys[i].SetActive(true);
            replys[i].transform.Find("Prompt Text").GetComponent<TMP_Text>().text = currentInteraction.responses[i].prompt;
        }
        for (int i=currentInteraction.responses.Length; i<replys.Length; i++)
        {
            replys[i].SetActive(false);
        }
    }
}
