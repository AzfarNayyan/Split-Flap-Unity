using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoPanel;

    private void Start()
    {
        // videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
        // videoPlayer.Prepare();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Invoke(nameof(wait), 8f);
        Invoke("HideIntro", 6.5f);
    }

    private void HideIntro()
    {
        videoPanel.SetActive(false);
    }

    void wait()
    {
        GetData.instance.StartDownloadData();
        SplitFlapController.instance.StartUpBoard(7, 22);
    }

}
 