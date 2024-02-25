using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

public class GetData : MonoBehaviour
{
    public static GetData instance;
    public int URLIndexWithData;
    public int urlIndex = 0;
    public List<string> URL = new List<string>();
    public bool isReadErrorScreen;


    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class Page
    {
        public string ScriptName;
        public int audio;
        public string backgroundframe;
        public string backgroundupper;
        public string backgroundlower;
        public string foreground;
        public string characters;
        public string messages;
        public string attributes;
        public float Duration;
        public int silent;
        public float refreshrate;
        public int rows;
        public int columns;
    }
    public int pageCount;
    public int currentPageIndex = 0;
    public Page currentPage = new Page();

    public void Start()
    {
        for (int i = 1; i <= 10; i++)
        {
            URL.Add("https://tv0" + i + ".splitflaptv.com/data/" + SystemInfo.deviceUniqueIdentifier + "/display.json");//" + SystemInfo.deviceUniqueIdentifier + "
        }
        
    }

    public void ReadData()
    {
        isReadErrorScreen = false;

        string filePath = Path.Combine(Application.persistentDataPath, "Data.json");
        if (File.Exists(filePath))
        {

            string jsonContent = File.ReadAllText(filePath);
            JSONArray jsonDataArray = JSON.Parse(jsonContent).AsArray;
            pageCount = jsonDataArray.Count;
            if (currentPageIndex >= pageCount)
            {
                currentPageIndex = 0;
            }
            

            if(pageCount==0)
            {
                SplitFlapController.instance.StartUpBoard(7, 22);
                Invoke(nameof(StartDownloadData), 30);
            }
            else
            {
                currentPage = JsonUtility.FromJson<Page>(jsonDataArray[currentPageIndex].ToString());
                SplitFlapController.instance.Setdata();
            }
        }

    }

    public void ReadError()
    {
        isReadErrorScreen = true;

        string jsonContent = Resources.Load<TextAsset>("Error").ToString();
        JSONArray jsonDataArray = JSON.Parse(jsonContent).AsArray;
        pageCount = jsonDataArray.Count;
        currentPage = JsonUtility.FromJson<Page>(jsonDataArray[0].ToString());

        SplitFlapController.instance.Setdata();
    }

    public IEnumerator DownloadJSONCoroutine()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(URL[urlIndex]))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check if there are any errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error while downloading JSON: " + webRequest.error);
                if (urlIndex < 9)
                {
                    urlIndex++;
                    StartCoroutine(DownloadJSONCoroutine());
                    yield break;
                }
                else
                { 
                    if (!File.Exists(Path.Combine(Application.persistentDataPath, "Data.json")))
                    {
                        ReadError();
                        yield return new WaitForSeconds(30f);
                        urlIndex = 0;
                        StartCoroutine(DownloadJSONCoroutine());
                    }
                    else
                    {
                        //currentPageIndex = 0;
                        ReadData();
                    }
                }
            }
            else
            {
                URLIndexWithData = urlIndex;
                currentPageIndex = 0;

                // Save the downloaded JSON data to persistent data path
                string filePath = Path.Combine(Application.persistentDataPath, "Data.json");
                File.WriteAllText(filePath, webRequest.downloadHandler.text);
                Debug.Log("JSON file downloaded and saved to: " + filePath);
                ReadData();
            }
        }
    }

    public void StartDownloadData()
    {
        StartCoroutine(DownloadJSONCoroutine());
    }

}
