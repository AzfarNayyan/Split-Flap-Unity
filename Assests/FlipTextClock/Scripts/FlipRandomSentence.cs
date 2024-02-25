using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipRandomSentence : MonoBehaviour {
    public FlipBase[] countText;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartRandom()
    {
        for(int i=0;i<countText.Length;i++)
        {
            int num = Random.Range(1, countText[i].contentText.Length);
            countText[i].ChangeToCountIndex(num,false);
        }
    }
}
