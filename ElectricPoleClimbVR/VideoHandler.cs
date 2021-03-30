using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VideoHandler : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    public TMP_Text timeElapsedText;
    public TMP_Text videoLengthText;
    public Scrollbar videoScrollbar;

    public SteamVR_Action_Boolean videoSkipAction;

    private float speed = 10f;

    public

    // Start is called before the first frame update
    void Start()
    {
        SetVideoLength();

        StartCoroutine(ActivateToolTips());
    }

    IEnumerator ActivateToolTips()
    {
        yield return null;
        ControllerButtonHints.ShowButtonHint(Player.instance.leftHand, videoSkipAction);
        ControllerButtonHints.ShowTextHint(Player.instance.leftHand, videoSkipAction, "<<");

        ControllerButtonHints.ShowButtonHint(Player.instance.rightHand, videoSkipAction);
        ControllerButtonHints.ShowTextHint(Player.instance.rightHand, videoSkipAction, ">>");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateTimeElapsed();

        if (videoSkipAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            Rewind();
        }

        if (videoSkipAction.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            FastForward();
        }

    }

    private void FastForward()
    {
        videoPlayer.time += speed;
    }

    private void Rewind()
    {
        videoPlayer.time -= speed;
    }

    private void CalculateTimeElapsed()
    {
        string minutes = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
        string seconds = ((int)videoPlayer.time % 60).ToString("00");

        timeElapsedText.text = minutes + ":" + seconds;

        videoScrollbar.size = (float)(videoPlayer.time/videoPlayer.clip.length);
    }

    private void SetVideoLength()
    {
        string minutes = Mathf.Floor((int)videoPlayer.clip.length / 60).ToString("00");
        string seconds = ((int)videoPlayer.clip.length % 60).ToString("00");

        videoLengthText.text = minutes + ":" + seconds;
                
    }
}
