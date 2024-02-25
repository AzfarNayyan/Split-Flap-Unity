using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipBase : MonoBehaviour {

    public string[] contentText;
    public float changeOrderAngle = 270;

     float flipNormalSpeed = 800;
     float flipFastSpeed = 1600;
    bool isNormalSpeed = false;

    public GameObject countDownPrefab;

    public int autoCountDownTimes = 0;
    int currentCountDownTime = 0;

    GameObject[] numberObjects;


    bool isCountDown = false;

    float currentAngle = 360;
    float nextAngle = 180;
    float shadowAlpha = 0;

    int currentNumber = 0;
    int nextNumber = 0;

    Transform currentUpUI;
    Transform nextDownUI;

    Image currentUpShadow;
    Image currentDownShadow;

    Vector3 xVec = new Vector3(1, 0, 0);
    Color restoreColor = new Color(1, 1, 1, 0);

    // Use this for initialization
    void Start () {
        numberObjects = new GameObject[contentText.Length];

        for(int i=numberObjects.Length-1;i>=0 ;i--)
        {
            numberObjects[i] = Instantiate(countDownPrefab);
            numberObjects[i].transform.position = this.transform.position;

            numberObjects[i].transform.SetParent(this.transform);
            
            numberObjects[i].name = "Countdown_" + contentText[i];

            numberObjects[i].transform.localScale = new Vector3(1, 1, 1);

            Transform findText = numberObjects[i].transform.Find("Up/Text");
            if (findText!=null)
                findText.GetComponent<Text>().text = contentText[i];
            findText = numberObjects[i].transform.Find("Down/Text");
            if (findText != null)
                findText.GetComponent<Text>().text = contentText[i];

        }

        if (autoCountDownTimes > 0)
        {
            currentCountDownTime = 1;
            StartCountDown();
        }
    }
	
	// Update is called once per frame
	void Update () {
        //if (countDownTimes>0 && currentCountDownTime<countDownTimes)
        //{
        //    isCountDown = true;
        //}


		if (isCountDown)
        {
            float speed = flipNormalSpeed;
            if (!isNormalSpeed)
                speed = flipFastSpeed;

            currentAngle -= Time.deltaTime * speed;
            nextAngle -= Time.deltaTime * speed;

            //进行到一半
            if (currentAngle< changeOrderAngle)
            {
                //把当前的移动到最后面
                numberObjects[nextNumber].transform.SetSiblingIndex(-1);
            }

            //翻页结束
            if (currentAngle <= 180)
            {
                isCountDown = false;

                currentUpShadow.color = restoreColor;
                currentDownShadow.color = restoreColor;

                numberObjects[currentNumber].transform.SetAsFirstSibling();

                currentUpUI.rotation = Quaternion.AngleAxis(0, xVec);
                nextDownUI.rotation = Quaternion.AngleAxis(0, xVec);

                currentNumber += 1;

                if (currentNumber >= numberObjects.Length)
                {
                    currentNumber = 0;
                }

                if (autoCountDownTimes > 0)
                {
                    currentCountDownTime += 1;
                    if (currentCountDownTime < autoCountDownTimes)
                        StartCountDown();
                    else
                    {
                        //结束
                        isNormalSpeed = true;
                    }
                }

                //currentAngle = 180;
                //nextAngle = 180;

                //currentUpUI.rotation = Quaternion.AngleAxis(currentAngle, xVec);

                return;
            }
            currentUpUI.rotation = Quaternion.AngleAxis(currentAngle, xVec);
            nextDownUI.rotation = Quaternion.AngleAxis(nextAngle, xVec);

            shadowAlpha += 1f;

            Color c = new Color(1, 1, 1, shadowAlpha / 30f);

            currentUpShadow.color = c;
            currentDownShadow.color = c;

        }
    }

    //private void OnGUI()
    //{

    //    if (GUI.Button(new Rect(0, 0, 130, 35), "Next Number"))
    //    {
    //        StartCountDown();
    //    }
    //}

    public void StartCountDown()
    {
        if (isCountDown)
            return;

        currentAngle = 360;
        nextAngle = 180;
        shadowAlpha = 0;
        isCountDown = true;
        FindScrollObject();
    }

    //public void StartAutoCountDown()
    //{
    //    ChangeToCountIndex(0);
    //    if (autoCountDownTimes > 0)
    //    {
    //        currentCountDownTime = 1;
    //        StartCountDown();
    //    }
    //}


    public void ChangeToCountText(string text,bool isFastSpeed)
    {
        for(int i=0;i<contentText.Length;i++)
        {
            if (contentText[i] == text)
                ChangeToCountIndex(i, isFastSpeed);
        }
    }


    public void ChangeToCountIndex(int index,bool isFastSpeed)
    {
        if (currentNumber == index)
            return;

        if (isFastSpeed)
            isNormalSpeed = false;
        else
            isNormalSpeed = true;

        if (index>currentNumber)
        {
            //直接跳转到对应页面
            autoCountDownTimes = index - currentNumber;
            currentCountDownTime = 0;
            StartCountDown();
        }
        else
        {
            //要计算一个周期
            autoCountDownTimes = numberObjects.Length - currentNumber + index;
            currentCountDownTime = 0;
            StartCountDown();
        }

    }

    public int GetCurrentNumberIndex()
    {
        return currentNumber;
    }

    public string GetCurrentText()
    {
        return contentText[currentNumber];
    }



    void FindScrollObject()
    {
        nextNumber = currentNumber + 1;
        if (nextNumber >= numberObjects.Length)
            nextNumber = 0;

        currentUpUI = numberObjects[currentNumber].transform.Find("Up");
        nextDownUI = numberObjects[nextNumber].transform.Find("Down");

        nextDownUI.rotation = Quaternion.AngleAxis(180, xVec);

        Transform upShadow = numberObjects[currentNumber].transform.Find("Up/Shadow");

        Transform downShadow = numberObjects[currentNumber].transform.Find("Down/Shadow");

        if (upShadow != null)
            currentUpShadow = upShadow.gameObject.GetComponent<Image>();
        if (downShadow != null)
            currentDownShadow = downShadow.gameObject.GetComponent<Image>();
    }
}
