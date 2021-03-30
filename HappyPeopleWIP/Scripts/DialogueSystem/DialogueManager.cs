using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.Playables;
using MoreMountains.TopDownEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;                                 //DialogueManager is a singleton

    [SerializeField]
    private float delayBetweenEmojis = 2f;                                  //How long to wait between emojis in the same speechbubble

    [SerializeField]
    private float delayBetweenBubbles = 3;                                  //How long to wait between speechbubbles in the same conversation

    [SerializeField]
    private PlayableDirector playableDirector;                              //Cinemachines playableDirector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //Shows the dialogue from SpeechBubble, called from Fungus
    public void ShowDialogue(SpeechBubbleDisplay character, SpeechBubble speechBubble, Flowchart thisFlowchart)
    {
        //Calls different functions depending on SpeechBubbles BubbleType
        switch (speechBubble.bubbleType)
        {
            case SpeechBubble.BubbleType.CharacterSpeaks:
                StartCoroutine(DisplayBubble(character, speechBubble , thisFlowchart));
                break;
            case SpeechBubble.BubbleType.PlayersChoice:
                DisplayPlayerChoices(character, speechBubble);
                break;
            default:
                break;
        }
    }

    //Displays the SpeechBubble according to number of emojis and sets the WaitTime -variable in Fungus to match delayBetweenBubbles
    private IEnumerator DisplayBubble(SpeechBubbleDisplay character, SpeechBubble speechBubble, Flowchart flowchart)
    {
        switch (speechBubble.emojiSprites.Count)
        {
            case 1:
                flowchart.SetFloatVariable("WaitTime", delayBetweenBubbles);
                //character.SetBubbleScale(1);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[0].sprite, 1));
                break;
            case 2:
                flowchart.SetFloatVariable("WaitTime", delayBetweenBubbles * 2);
                //character.SetBubbleScale(2);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[0].sprite, 1));
                yield return new WaitForSeconds(delayBetweenEmojis);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[1].sprite, 2));
                break;
            case 3:
                flowchart.SetFloatVariable("WaitTime", delayBetweenBubbles * 3);
                //character.SetBubbleScale(3);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[0].sprite, 1));
                yield return new WaitForSeconds(delayBetweenEmojis);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[1].sprite, 2));
                yield return new WaitForSeconds(delayBetweenEmojis);
                yield return StartCoroutine(character.ShowBubble(speechBubble.bubbleSprites[0].sprite, speechBubble.emojiSprites[2].sprite, 3));
                break;
            default:
                break;
        }
    }    
    //Shows PlayerChoice -speecbubble
    private void DisplayPlayerChoices(SpeechBubbleDisplay character, SpeechBubble speechBubble)
    {
        character.ShowChoices(speechBubble);
    }

    //Sets cinemachines speed to 0 or 1 depending on boolean value
    public void PauseCinemachine(bool on)
    {
        if (on)
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }

        else
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }

    //Sets cinemachines playableDirector from the parent object
    public void SetPlayableDirector(GameObject eventParentObject)
    {
        playableDirector = eventParentObject.GetComponent<PlayableDirector>();
    }

    //Turns TopDownController3D-script on/off, allows moving player character with Fungus' MoveTo-function when turned off 
    public void HandleTopDownController(bool disable, TopDownController3D topDownController3D)
    {
        if (disable)
        {
            topDownController3D.enabled = false;

        }
        else
        {
            topDownController3D.enabled = true;
        }
    }
}
