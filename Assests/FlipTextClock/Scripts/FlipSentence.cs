using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipSentence : MonoBehaviour {

    public FlipBase[] countText;
    public InputField inputText;

	// Use this for initialization
	void Start () 
    {
		for (int i = 0;i<160;i++)
        {
            countText[i]=transform.GetChild(i).GetComponent<FlipBase>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetText()
    {
        SetText(inputText.text);
    }

    public void SetText(string text)
    {
        string str = text;
        if (text.Length > countText.Length)
            str = text.Substring(0, countText.Length);
        
        if (str.Length<countText.Length)
        {
            //补空格
            for(int i=str.Length;i<countText.Length;i++)
            {
                str = str + " ";
            }
        }

        str = str.ToUpper();

        for(int i=0;i<countText.Length;i++)
        {
            string c = str.Substring(i, 1);
            countText[i].ChangeToCountText(c,false);
        }
    }
}
