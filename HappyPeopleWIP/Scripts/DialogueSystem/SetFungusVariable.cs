using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class SetFungusVariable : MonoBehaviour
{
    //This script was for testing how to change Fungus variables

    [SerializeField]
    private SpeechBubble speechBubble;

    private string fungusString;

    [SerializeField]
    private Flowchart flowchart;

    // Start is called before the first frame update
    void Start()
    {
        fungusString = speechBubble.bubbleType.ToString();
    }

    public void FungusVariableSetter()
    {
        flowchart.SetStringVariable("dialogueType", fungusString);

    }
    
}
