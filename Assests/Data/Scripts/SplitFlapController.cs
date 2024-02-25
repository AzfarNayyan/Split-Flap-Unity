using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using ColorUtility = UnityEngine.ColorUtility;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using System.Data.Common;
using System.Runtime.InteropServices.WindowsRuntime;

public class SplitFlapController : MonoBehaviour
{
    public static SplitFlapController instance;

    public GameObject Alphabet;
    public GetData Data;
    public Image backgroundFrame;
    public List<GameObject> Alphabets;
    public List<AudioClip> AudioClipList;
    public List<string> Attributes;
    public float delay = 0.05f;
    public string charachters;
    public string messages;
    public RectTransform rectTransform;
    public GridLayoutGroup grid;
    //PREVIOUS DATA
    public int previousRows = 7;
    public int previousCols = 22;
    public string previousBackgroundFrame;
    public string previousUpper;
    public string previousLower;
    public string previousForeground;
    public string previousCharacters;
    public string previousMessages;
    public bool ClearBoard = false;
    public bool DataUpdated;

    //NEW
    List<int> Index = new List<int>();
    //END


    [SerializeField]
    List<Vector2> RC = new List<Vector2>();

    private void Awake()
    {
        instance = this;
    }

    public void Setdata()
    {
        RC ??= new();
        RC.Clear();

        GetComponent<AudioSource>().volume = 1;
        GetComponent<AudioSource>().clip = AudioClipList[Data.currentPage.audio - 1];

        if (previousRows != Data.currentPage.rows || previousCols != Data.currentPage.columns)
        {
            ClearBoard = true;

            backgroundFrame.color = ColorUtility.TryParseHtmlString(previousBackgroundFrame, out Color colorResult) ? colorResult : Color.white;

            charachters = previousCharacters;
            if (!charachters.Contains(' '))
                charachters =charachters+' ';
            if (!charachters.Contains('\n'))
                charachters = '\n' + charachters;
            if (!charachters.Contains('*'))
                charachters = charachters + '*';
            charachters = charachters + "abcdefghijklmnopqrstuvwxyz";

            SetBoard(previousRows, previousCols);

            messages = "";

            int r = 0;
            int c = 0; ;
            for (int z = 0; z < messages.Length; z++)
            {
                bool skip = false;

                if (messages[z] == '\n')
                {
                    skip = true;
                    r++;
                    c = -1;
                }
                else if (c == previousCols)
                {
                    r++;
                    c = 0;
                }

                if (r == previousRows)
                {
                    break;
                }

                if (!skip)
                    RC.Add(new Vector2(r, c));

                c++;
            }

            messages = messages.Replace("\n", "");
            messages = messages.Substring(0, RC.Count);
            StartCoroutine(ChangeAlphabetsOverTime(2, previousRows, previousCols, charachters));
            
        }
        else
        {
            SetUpdatedData();
        }
    }

    private IEnumerator ChangeAlphabetsOverTime(float duration, int Rows, int Cols, string CHARACTERS)
    {
        if (DataUpdated == true)
        {
            GetComponent<AudioSource>().volume = 1;
            float v = 0f;
            if (Data.currentPage.silent != 1)
            {
                v = (GetComponent<AudioSource>().volume / (Rows * Cols));
                GetComponent<AudioSource>().volume = v * messages.Length;
                GetComponent<AudioSource>().Play();
            }


            for (int a = 0; a < CHARACTERS.Length; a++)
            {
                bool isChanged = false;

                for (int i = 0; i < messages.Length; i++)
                {
                    int boxNo = (int)((RC[i].x * Cols) + RC[i].y);

                    Alphabets[boxNo].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = ColorUtility.TryParseHtmlString(Attributes[(int)(RC[i].x)], out Color colorResult3) ? colorResult3 : Color.white;
           

                    if (!Alphabets[boxNo].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text.Equals(messages[i].ToString()))
                    {
                        isChanged = true;
                        Alphabets[boxNo].transform.GetChild(3).gameObject.SetActive(true);

                        Alphabets[boxNo].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = CHARACTERS[a].ToString();

                        if (Alphabets[boxNo].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text.Equals(messages[i].ToString()))
                        {
                            GetComponent<AudioSource>().volume -= v;
                        }

                        if (a == charachters.Length - 1 && !Alphabets[boxNo].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text.Equals(messages[i].ToString()))
                        {
                            Alphabets[boxNo].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "*";
                            Alphabets[boxNo].transform.GetChild(3).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Alphabets[boxNo].transform.GetChild(3).gameObject.SetActive(false);
                    }
                }

                if (!isChanged)
                {
                    GetComponent<AudioSource>().Stop();
                    break;
                }

                yield return new WaitForSeconds(delay);
            }
        }

        yield return new WaitForSeconds(duration);

        if (!ClearBoard)
        {
            Data.currentPageIndex++;
         
            if (Data.currentPageIndex < Data.pageCount)
            {
                //Data.currentPageIndex++;
                Data.ReadData();
            }
            else
            {
               
                yield return new WaitForSeconds(Data.currentPage.refreshrate);
                Data.urlIndex = Data.URLIndexWithData;
                if (!Data.isReadErrorScreen)
                {
                    Data.StartDownloadData();
                }
            }
        }
        else
        {
            SetUpdatedData();
        }
    }

    private void SetBoard(int Rows, int Cols)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Alphabets.Clear();

        float parentWidth = rectTransform.rect.width - (grid.spacing.x * (Cols + 2)) - grid.padding.left;
        float parentHeight = rectTransform.rect.height - (grid.spacing.y * (Rows + 1)) - grid.padding.top;

        float cellWidth = (parentWidth / (float)Cols);
        float cellHeight = (parentHeight / (float)Rows);

        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellWidth, cellHeight);

        for (int i = 0; i < (Rows * Cols); i++) 
        {
            GameObject A = Instantiate(Alphabet, transform);
            Alphabets.Add(A);
            Alphabets[i].transform.GetChild(0).GetComponent<Image>().color = ColorUtility.TryParseHtmlString(Data.currentPage.backgroundupper, out Color colorResult1) ? colorResult1 : Color.grey;
            Alphabets[i].transform.GetChild(1).GetComponent<Image>().color = ColorUtility.TryParseHtmlString(Data.currentPage.backgroundlower, out Color colorResult2) ? colorResult2 : Color.grey;
            Alphabets[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = ColorUtility.TryParseHtmlString(Data.currentPage.foreground, out Color colorResult3) ? colorResult3 : Color.white;
        }

        previousRows = Rows;
        previousCols = Cols;
        previousBackgroundFrame = Data.currentPage.backgroundframe;
        previousUpper = Data.currentPage.backgroundupper;
        previousLower = Data.currentPage.backgroundlower;
        previousForeground = Data.currentPage.foreground;
        previousCharacters = Data.currentPage.characters;
    }

    public void StartUpBoard(int Rows, int Cols)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Alphabets.Clear();

        float parentWidth = rectTransform.rect.width - (grid.spacing.x * (Cols + 2)) - grid.padding.left;
        float parentHeight = rectTransform.rect.height - (grid.spacing.y * (Rows + 1)) - grid.padding.top;

        float cellWidth = (parentWidth / (float)Cols);
        float cellHeight = (parentHeight / (float)Rows);

        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellWidth, cellHeight);

        for (int i = 0; i < (Rows * Cols); i++)
        {
            GameObject A = Instantiate(Alphabet, transform);
            Alphabets.Add(A);
        }
    }

    private void SetUpdatedData()
    {
        if (previousMessages != Data.currentPage.messages)
        {
            DataUpdated = true;
        }
        else
            DataUpdated = false;

        previousMessages = Data.currentPage.messages;

        RC ??= new();
        RC.Clear();

        ClearBoard = false;

        backgroundFrame.color = ColorUtility.TryParseHtmlString(Data.currentPage.backgroundframe, out Color colorResult) ? colorResult : Color.white;

        charachters = Data.currentPage.characters;
        if (!charachters.Contains(' '))
            charachters = ' ' + charachters;
        if (!charachters.Contains('\n'))
            charachters = '\n' + charachters;
        if (!charachters.Contains('*'))
            charachters = charachters + '*';
        charachters = charachters + "abcdefghijklmnopqrstuvwxyz";

        if (DataUpdated == true)
            SetBoard(Data.currentPage.rows, Data.currentPage.columns);

        messages = Data.currentPage.messages;

        //Replace ShortCodes
        messages = messages.Replace("#TM01#", System.DateTime.Now.ToString("HH:mm:ss"));
        messages = messages.Replace("#TM02#", System.DateTime.Now.ToShortTimeString());

        messages = messages.Replace("#DT01#", DateTime.Now.ToString("dd/MM/yyyy"));
        messages = messages.Replace("#DT02#", DateTime.Now.ToString("MM/dd/yyyy"));
        messages = messages.Replace("#DT03#", DateTime.Now.ToString("dd/MM/yy"));
        messages = messages.Replace("#DT04#", DateTime.Now.ToString("MM/dd/yyyy"));
        messages = messages.Replace("#DT05#", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
        messages = messages.Replace("#DT06#", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));

        messages = messages.Replace("#DEVICEID#", SystemInfo.deviceUniqueIdentifier);
        messages = messages.Replace("#TIMEZONE#", TimeZoneInfo.Local.StandardName);

        int r = 0;
        int c = 0; ;
        for (int z = 0; z < messages.Length; z++)
        {
            bool skip = false;

            if (messages[z] == '\n')
            {
                skip = true;
                r++;
                c = -1;
            }
            else if (c == Data.currentPage.columns)
            {
                r++;
                c = 0;
            }

            if (r == Data.currentPage.rows)
            {
                break;
            }

            if (!skip)
                RC.Add(new Vector2(r, c));

            c++;
        }

        SaveAttributes();
        messages = messages.Replace("\n", "");
        messages = messages.Substring(0, RC.Count);
        StartCoroutine(ChangeAlphabetsOverTime(Data.currentPage.Duration, Data.currentPage.rows, Data.currentPage.columns, charachters));
        
    }

    private void SaveAttributes()
    {
        Attributes.Clear();
        char[] separators = { '\n', '#' };
        string[] dataArray = Data.currentPage.attributes.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

        int j = 0;
        int k = 0;

        for (int i = 0; i < Data.currentPage.attributes.Length; i++) 
        {
            if (i == 0 && Data.currentPage.attributes[i] == '\n') 
            {
                Attributes.Add(Data.currentPage.foreground);
                k++;   
            }
            else if (Data.currentPage.attributes[i] == '\n' && Data.currentPage.attributes[i - 1] == '\n') 
            {
                Attributes.Add(Data.currentPage.foreground);
                k++;
            }
            else if (Data.currentPage.attributes[i]=='#')
            {
                Attributes.Add('#' + dataArray[j]);
                k++;
                j++;
            }
        }

        for (int x = k; x < Data.currentPage.rows; x++)
            Attributes.Add(Data.currentPage.foreground);

    }

    
}

