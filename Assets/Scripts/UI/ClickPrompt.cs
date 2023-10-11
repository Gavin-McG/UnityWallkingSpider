using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPrompt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int promptNum;

    private bool mouseOver = false;

    private static DialogueManager dm;

    // Start is called before the first frame update
    void Start()
    {
        dm = GameObject.Find("Game Manager").GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseOver && Input.GetMouseButtonUp(0))
        {
            dm.choosePath(promptNum);
            //Debug.Log("clicked on prompt num " + promptNum);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        //Debug.Log("cursor entering " + name + " " + promptNum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        //Debug.Log("cursor exiting " + name + " " + promptNum);
    }
}
