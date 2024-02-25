using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTimer : MonoBehaviour {
    public FlipBase timerHigh;    //3
    public FlipBase timerLow;

    float tick = 0;
    bool isStart = false;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (isStart == false)
            return;

        tick += Time.deltaTime;

        int iCount = (int)tick;

        if (iCount > 99)
        {
            iCount = 0;
            tick = 0;
            //isStart = false;
        }

        if (iCount > 9)
        {
            string highStr = iCount.ToString().Substring(0, 1);
            string lowStr = iCount.ToString().Substring(1, 1);

            timerHigh.ChangeToCountText(highStr, false);
            timerLow.ChangeToCountText(lowStr, false);
        }
        else
        {
            string lowStr = iCount.ToString();

            timerHigh.ChangeToCountText("0", false);
            timerLow.ChangeToCountText(lowStr, false);
        }
    }

    public void StartTimer()
    {
        isStart = true;
    }

    public void PauseTimer()
    {
        isStart = false;
    }

    public void ResetTimer()
    {
        isStart = false;
        tick = 0;
        timerHigh.ChangeToCountText("0", true);
        timerLow.ChangeToCountText("0", true);
    }
}
