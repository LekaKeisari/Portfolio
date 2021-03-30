using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New DialogueEvent", menuName = "DialogueEvent")]
public class DialogueEvent : ScriptableObject
{
    //This is the first version of the dialogue scriptable object, not used anymore

    [HelpBox("Insert emoji-sprites to the lists below. 0 = positive, 1 = neutral, 2 = negative. Player's choice is displayed in this order", messageType = HelpBoxMessageType.Info)]
    [Header("Emojis")]
    public List<Sprite> playerEmojiSprites = new List<Sprite>();
    public List<Sprite> nPCEmojiSprites = new List<Sprite>();
    
    
    [HelpBox("Insert speechbubble-sprites below. 0 = default", messageType = HelpBoxMessageType.Info)]
    [Header("Bubbles")]
    public List<Sprite> playerBubbleSprites = new List<Sprite>();
    public List<Sprite> nPCBubbleSprites = new List<Sprite>();

    public enum DialogueFlow
    {
        PlayersChoice,
        NPCSpeaksToPlayer,
        NPCSpeaksToNPC,
        PlayerSpeaksToNPC
    }

    [HelpBox("Set the order and number of events in dialogue below", messageType = HelpBoxMessageType.Info)]
    [Header("Dialogue FlowChart")]
    public List<DialogueFlow> dialogueFlowChart = new List<DialogueFlow>();

    [HelpBox("Set a number to match the desired responce to player's choice. For example number 1 in slot 0: player selects emoji 0 and NPC answers with emoji 1", messageType = HelpBoxMessageType.Info)]
    [Header("NPC speaks to Player")]
    public List<int> nPCAnswers = new List<int>();
    
    [HelpBox("Each NPC says 1 emoji at a time in the order of the list. Example: 3 NPCs talking, first one says emoji at 0, second says 1 and third says 2, and first says 3", messageType = HelpBoxMessageType.Info)]
    [Header("NPC speaks to NPC")]
    public List<int> nPCConversation = new List<int>();

    [HelpBox("Set a number to match the desired responce to NPCs answer. For example number 1 in slot 0: NPC says emoji 0 and Player answers with emoji 1", messageType = HelpBoxMessageType.Info)]
    [Header("Player speaks to NPC")]
    public List<int> playerAnswers = new List<int>();


    void Start()
    {

    }
}
