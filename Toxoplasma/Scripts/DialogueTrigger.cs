using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public int dialogueCounter = 0;
    [HideInInspector]
    public bool dialoguePossible = false;

    [HideInInspector]
    public bool callFromEvent = false;

    [HideInInspector]
    public bool sentenceDone = false;

    //[SerializeField]
    public DialogueManager dialogueManager;

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

    public void TriggerNextSentence()
    {
        dialogueManager.DisplayNextSentence();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.name == "Player")
        {
            dialoguePossible = true;
            StartCoroutine(PrintDialogue());
        }                   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            dialogueCounter = 0;
            dialoguePossible = false;
        }
    }

    public IEnumerator PrintDialogue()
    {
        while (dialoguePossible)
        {
            if (Input.GetKeyDown(KeyCode.E) && dialogueManager.typingDone)
            {
                if (dialogueCounter == 0)
                {
                    dialogueManager.firstSentence = true;
                    TriggerDialogue();
                    dialogueCounter++;
                }
                else
                {
                    dialogueManager.firstSentence = false;
                    TriggerNextSentence();
                    dialogueCounter++;
                }
            }
            else if (callFromEvent && dialogueManager.typingDone)
            {
                if (dialogueCounter == 0)
                {
                    dialogueManager.firstSentence = true;

                    TriggerDialogue();
                    dialogueCounter++;
                }
                else
                {
                    dialogueManager.firstSentence = false;
                    yield return new WaitUntil(() => Input.GetKey(KeyCode.E));
                    sentenceDone = true;
                    TriggerNextSentence();
                    dialogueCounter++;                    
                }
            }
            yield return new WaitForSeconds(0.001f);
            sentenceDone = false;
        }
        yield return null;
    }
}
