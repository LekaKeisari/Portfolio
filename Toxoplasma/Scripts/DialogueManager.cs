using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public RectTransform dialogueImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public CameraMovement mainCam;

    public bool typingDone = true;
    public bool dialogueDone = true;
    public bool firstSentence;

    public Animator textAnimator;
    
    [SerializeField]
    private GameManager gameManager;

    private Queue<string> sentences;

    private Coroutine TypingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        sentences = new Queue<string>();
        gameManager.dialogueManager = this;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //gameManager.playerMoveAllowed = false;
        if (gameManager.dangerZone)
        {
            gameManager.ToggleRandomBattle(false);
        }
        dialogueDone = false;
        dialogueCanvas.SetActive(true);
        //textAnimator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        //Debug.Log("Dialogue name " + dialogue.name);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        if (TypingCoroutine != null)
        {
            StopCoroutine(TypingCoroutine);
        }
        TypingCoroutine = StartCoroutine(TypeSentence(sentence));
        //Debug.Log(sentence);
    }

    private IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        typingDone = false;
        if (!firstSentence || Input.GetKeyDown(KeyCode.E))
        {
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));

        }

        foreach (char letter in sentence.ToCharArray())
        {
            if (!Input.GetKey(KeyCode.E))
            {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);

            }
            else
            {
                dialogueText.text = sentence;
                StartCoroutine(SkipTypingHelper());
                if (TypingCoroutine != null)
                {
                    StopCoroutine(TypingCoroutine);
                }
                yield return null;
            }
        }
        typingDone = true;
    }

    private void EndDialogue()
    {
        //textAnimator.SetBool("IsOpen", false);
        dialogueCanvas.SetActive(false);
        dialogueDone = true;
        //gameManager.playerMoveAllowed = true;

        if (gameManager.dangerZone && !gameManager.cameraFollowDisabled)
        {
            gameManager.ToggleRandomBattle(true);
        }

        //Debug.Log("End of dialogue");
    }

    private IEnumerator SkipTypingHelper()
    {
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
        typingDone = true;
    }
        
}
