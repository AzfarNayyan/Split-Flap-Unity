using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipClock : MonoBehaviour {

    public FlipBase hourHigh;
    public FlipBase hourLow;

    public FlipBase minuteHigh;
    public FlipBase minuteLow;

    public FlipBase secondHigh;
    public FlipBase secondLow;

    //float tick = 0;
    //bool isInstallTimer = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        //tick += Time.deltaTime;
        //if (isInstallTimer == false)
        //{
        //    if (tick > 1.5f)
        //    {
        //        ChangeToCurrentTime(true);
        //        isInstallTimer = true;
        //    }
        //}
        //else
        //{
            ChangeToCurrentTime();
        //}

    }

    void ChangeToCurrentTime()
    {
        int h = DateTime.Now.Hour;
        int m = DateTime.Now.Minute;
        int s = DateTime.Now.Second;

        if (m > 9)
        {
            string highStr = m.ToString().Substring(0, 1);
            string lowStr = m.ToString().Substring(1, 1);

            minuteHigh.ChangeToCountText(highStr, false);
            minuteLow.ChangeToCountText(lowStr, false);
        }
        else
        {
            string lowStr = m.ToString();
            minuteHigh.ChangeToCountText("0", false);
            minuteLow.ChangeToCountText(lowStr, false);
        }

        if (h > 9)
        {
            string highStr = h.ToString().Substring(0, 1);
            string lowStr = h.ToString().Substring(1, 1);

            hourHigh.ChangeToCountText(highStr, false);
            hourLow.ChangeToCountText(lowStr, false);
        }
        else
        {
            string lowStr = h.ToString();
            hourHigh.ChangeToCountText("0", false);
            hourLow.ChangeToCountText(lowStr, false);
        }


        //秒有点复杂，需要判断当前秒和自己差多少，差了多了要加快翻页速度
        bool isFastSpeed = false;

        int currentSecond = Convert.ToInt32(secondHigh.GetCurrentText() + secondLow.GetCurrentText());
        if (Math.Abs(s - currentSecond) > 3)
            isFastSpeed = true;
        if (s == 59 || s == 60 || s==0)
            isFastSpeed = false;

        if (s>9)
        {
            //bool isFirst = false;
            //if (isInstall)
            //    isFirst = true;

            string highStr = s.ToString().Substring(0, 1);
            string lowStr = s.ToString().Substring(1, 1);

            secondHigh.ChangeToCountText(highStr, false);
            secondLow.ChangeToCountText(lowStr, isFastSpeed);
        }
        else
        {
            //bool isFirst = false;
            //if (isInstall)
            //    isFirst = true;

            string lowStr = s.ToString();
            secondHigh.ChangeToCountText("0", false);
            secondLow.ChangeToCountText(lowStr, isFastSpeed);
        }
    }

}
