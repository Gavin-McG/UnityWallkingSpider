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

    public Interaction currentInteraction;

    private string currentText = "";
    private float charDelay = 0.1f;
    private float textTime;

    // Update is called once per frame
    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentText.Length < currentInteraction.mainDialogue.Length)
                {
                    currentText = currentInteraction.mainDialogue;
                }
                else
                {
                    SetReplys();
                }
            }
            textTime += Time.fixedDeltaTime;
            while (currentText.Length < currentInteraction.mainDialogue.Length && charDelay > textTime)
            {
                currentText += currentInteraction.mainDialogue[currentText.Length];
                textTime -= charDelay;
            }
        }
    }

    public void ActivateDialogue(Interaction newInteration)
    {
        Debug.Assert(currentInteraction != null);

        currentInteraction = newInteration;

        Time.timeScale = 0.0f;
        textTime = 0.0f;
        mainBox.SetActive(true);
        dialogue.SetActive(true);
        prompts.SetActive(false);

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
            return;
        }

        Debug.Assert(currentInteraction.responses.Length <= replys.Length);

        for (int i=0; i<currentInteraction.responses.Length; i++)
        {
            replys[i].SetActive(true);
            replys[i].transform.Find("Prompt Text").GetComponent<TextMeshPro>().text = currentInteraction.responses[i].prompt;
        }
        for (int i=currentInteraction.responses.Length; i<replys.Length; i++)
        {
            replys[i].SetActive(false);
        }
    }

    public void choosePath(int opt)
    {
        Debug.Assert(opt < currentInteraction.responses.Length);
        ActivateDialogue(currentInteraction.responses[opt].reply);
    }
}
