using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpeechBubbleDisplay : MonoBehaviour
{       
    private Animator canvasAnimator;
    private Animator emoji1Animator;
    private Animator emoji2Animator;
    private Animator emoji3Animator;

    [Header("Canvases & images")]

    //Stuff below need to be dragged and dropped
    [SerializeField]
    private List<Image> speechBubbleImages = new List<Image>();

    [SerializeField]
    private List<Image> emojiImages = new List<Image>();
    
    public Canvas screenSpaceCanvas;
    public Canvas worldSpaceCanvas;
    
    [SerializeField]
    private Image worldSpaceBubble;
    [SerializeField]
    private Image worldSpaceEmoji1;
    [SerializeField]
    private Image worldSpaceEmoji2;
    [SerializeField]
    private Image worldSpaceEmoji3;
    
    void Start()
    {
        canvasAnimator = worldSpaceCanvas.GetComponent<Animator>();
        emoji1Animator = worldSpaceEmoji1.GetComponent<Animator>();
        emoji2Animator = worldSpaceEmoji2.GetComponent<Animator>();
        emoji3Animator = worldSpaceEmoji3.GetComponent<Animator>();
    }     

    //Shows the SpeechBubble and its emoji depending on emojiPosition which is the same as order number from left to right
    public IEnumerator ShowBubble(Sprite bubble, Sprite emoji, int emojiPosition)
    {
        worldSpaceCanvas.gameObject.SetActive(true);
        worldSpaceBubble.sprite = bubble;
        switch (emojiPosition)
        {
            case 1:
                worldSpaceEmoji1.sprite = emoji;

                canvasAnimator.SetTrigger("PopUp");                                                 //Triggers animation for bubble
                float animationLength = canvasAnimator.GetCurrentAnimatorStateInfo(0).length;       //checks animations length
                yield return new WaitForSeconds(animationLength);                                   //waits for the animations duration
                worldSpaceEmoji1.gameObject.SetActive(true);
                emoji1Animator.SetTrigger("PopUp");                                                 //Triggers animation for emoji
                break;
            case 2:
                worldSpaceEmoji2.sprite = emoji;
                worldSpaceEmoji2.gameObject.SetActive(true);
                emoji2Animator.SetTrigger("PopUp");                                                 //Triggers animation for emoji

                break;
            case 3:
                worldSpaceEmoji3.sprite = emoji;
                worldSpaceEmoji3.gameObject.SetActive(true);
                emoji3Animator.SetTrigger("PopUp");                                                 //Triggers animation for emoji

                break;
            default:
                break;
        }

        yield return null;
    }

    //Displays the emojichoices
    public void ShowChoices(SpeechBubble speechBubble)
    {
        //Loops through different bubbles and sets them active
        for (int i = 0; i < speechBubble.bubbleSprites.Count; i++)
        {
            speechBubbleImages[i].gameObject.SetActive(true);
            speechBubbleImages[i].sprite = speechBubble.bubbleSprites[i].sprite;
        }
        
        //Loops through different emojis and sets them active
        for (int i = 0; i < speechBubble.emojiSprites.Count; i++)
        {
            emojiImages[i].gameObject.SetActive(true);
            emojiImages[i].sprite = speechBubble.emojiSprites[i].sprite;
        }
        screenSpaceCanvas.gameObject.SetActive(true);
    }

    //Hides speechBubbles and emojis
    public IEnumerator HideBubble()
    {
        emoji1Animator.SetTrigger("Shrink");
        worldSpaceEmoji1.gameObject.SetActive(false);

        emoji2Animator.SetTrigger("Shrink");
        worldSpaceEmoji2.gameObject.SetActive(false);

        emoji3Animator.SetTrigger("Shrink");
        worldSpaceEmoji3.gameObject.SetActive(false);

        canvasAnimator.SetTrigger("Shrink");
        float animationLength = canvasAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength + .5f);

        worldSpaceCanvas.gameObject.SetActive(false);

        yield return null;

    }

    //Hides PlayersChoices by setting each speechbubble and emoji deactive
    public void HideChoices()
    {
        foreach (Image bubble in speechBubbleImages)
        {
            bubble.gameObject.SetActive(false);

        }

        foreach (Image emoji in emojiImages)
        {
            emoji.gameObject.SetActive(false);

        }

        screenSpaceCanvas.gameObject.SetActive(false);
    }

    //Scales speechbubbles depending on number of emojis
    public void SetBubbleScale(int numberOfEmojis)
    {
        Vector2 cellSize = worldSpaceBubble.GetComponentInChildren<GridLayoutGroup>().cellSize;
        Vector2 cellsSpacing = worldSpaceBubble.GetComponentInChildren<GridLayoutGroup>().spacing;
        worldSpaceBubble.rectTransform.sizeDelta = new Vector2((cellSize.x * 2 + cellsSpacing.x) * numberOfEmojis, cellSize.y * 2);
    }
}
