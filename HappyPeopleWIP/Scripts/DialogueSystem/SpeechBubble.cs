using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu(fileName = "New SpeechBubble", menuName = "SpeechBubble")]
public class SpeechBubble : ScriptableObject
{
    public enum BubbleType
    {
        CharacterSpeaks,
        PlayersChoice        
    }

    [HelpBox("How many emojis/bubbles/choices for this event", messageType = HelpBoxMessageType.Info)]
    [Header("Settings")]
    [Range(1,4)]
    public int emojis = 1;
    [Range(1,3)]
    public int bubbles = 1;
    [Range(2,3)]
    public int choices = 3;

    [HelpBox("Select SpeechBubble type", messageType = HelpBoxMessageType.Info)]
    [Header("Type")]
    public BubbleType bubbleType;

    [HelpBox("Insert emoji-sprites to the lists below.", messageType = HelpBoxMessageType.Info)]
    [Header("Emojis")]
    public List<SpriteWithPreview> emojiSprites = new List<SpriteWithPreview>();

    [HelpBox("Insert speechbubble-sprites below.", messageType = HelpBoxMessageType.Info)]
    [Header("Bubbles")]
    public List<SpriteWithPreview> bubbleSprites = new List<SpriteWithPreview>();

    //Updates when changes are made in editor
    private void OnValidate()
    {
        switch (bubbleType)
        {
            case BubbleType.CharacterSpeaks:                //Adds the correct amount of emoji-slots for the scriptable object
                if (emojiSprites.Count != emojis)
                {
                    emojiSprites.Clear();
                    while (emojiSprites.Count < emojis)
                    {
                        emojiSprites.Add(null);
                    }
                }

                if (bubbleSprites.Count != bubbles)         //Adds the correct amount of bubble-slots for the scriptable object
                {
                    bubbleSprites.Clear();
                    while (bubbleSprites.Count < bubbles)
                    {
                        bubbleSprites.Add(null);
                    }
                }

                break;
            case BubbleType.PlayersChoice:                  //Adds the correct amount of emoji-slots for the scriptable object
                if (emojiSprites.Count != choices)
                {
                    emojiSprites.Clear();
                    while (emojiSprites.Count < choices)
                    {
                        emojiSprites.Add(null);
                    }
                }

                if (bubbleSprites.Count != bubbles)         //Adds the correct amount of bubble-slots for the scriptable object
                {
                    bubbleSprites.Clear();
                    while (bubbleSprites.Count < bubbles)
                    {
                        bubbleSprites.Add(null);
                    }
                }

                break;           
            default:
                break;
        }
    }
}
