using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using ExtensionMethods;

public class GameManager : MonoBehaviour {

    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
           // testuje
            //Alternative Singleton pattern

            //if (_instance == null)
            //{
            //    _instance = FindObjectOfType<GameManager>();

            //    if (_instance == null)
            //    {
            //        GameObject container = new GameObject("GameManager");
            //        _instance = container.AddComponent<GameManager>();
            //    }
            //}

            return _instance;
        }
    }

    public GameObject blackField;
    public GameObject whiteField;
    public GameObject checkerRed;
    public GameObject checkerWhite;

    const int boardSize = 8;
    Checker [,] checkers;
    GameObject[,] boardField;
    static int curBoardPosX = 0;
    static int curBoardPosY = 0;

    public GameObject fullscreenLayer;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked += fullscreenFlickedHandler;
        generateBoard();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void fullscreenFlickedHandler(object sender, EventArgs e)
    {
        FlickGesture gesture = sender as FlickGesture;

        Vector2 dir = gesture.ScreenFlickVector;

        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            if (dir.x > 0)
            {
                Debug.Log("Swipe Right");
                curBoardPosX--;
            }
            else
            {
                Debug.Log("Swipe Left");
                curBoardPosX++;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                Debug.Log("Swipe Up");
                curBoardPosY--;
            }
            else
            {
                Debug.Log("Swipe Down");
                curBoardPosY++;
            }
        }

        curBoardPosX = curBoardPosX.Clamp(0, boardSize);
        curBoardPosY = curBoardPosY.Clamp(0, boardSize);
        Debug.Log(curBoardPosX + "," + curBoardPosY);
    }

    void onDestroy()
    {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked -= fullscreenFlickedHandler;
    }

    void generateBoard()
    {
        checkers = new Checker[boardSize, boardSize];
        boardField = new GameObject[boardSize, boardSize];
        GameObject boardObject =  Instantiate(new GameObject("Board"), gameObject.transform);
        boardObject.transform.localPosition = new Vector3(0, 0, 0.1f);
        GameObject checkerObject = Instantiate(new GameObject("Chekers"), gameObject.transform);
        checkerObject.transform.localPosition = new Vector3(0, 0, 0);

        bool color = true;  //black mean true, white false
        for (int i = 0; i < boardSize; i++)
        {
            color = !color;
            for (int j = 0; j < boardSize; j++)
            {
                color = !color;
                if (color)
                {
                    boardField[i, j] = Instantiate(blackField, boardObject.transform);
                    if (j < 3)//generate red
                    {
                    GameObject tmpObject = Instantiate(checkerRed, checkerObject.transform);
                    tmpObject.transform.localPosition = new Vector3(i, j, 0);
                    tmpObject.transform.SetParent(checkerObject.transform);
                    checkers[i,j] = tmpObject.GetComponent<Checker>();
                    }
                    if (boardSize-4<j)//generate white
                    {
                        GameObject tmpObject = Instantiate(checkerWhite, checkerObject.transform);
                        tmpObject.transform.localPosition = new Vector3(i, j, 0);
                        tmpObject.transform.SetParent(checkerObject.transform);
                        checkers[i, j] = tmpObject.GetComponent<Checker>();
                    }

                }
                else
                {
                    boardField[i, j] = Instantiate(whiteField, boardObject.transform);
                }
                boardField[i, j].transform.SetParent(boardObject.transform);
                boardField[i, j].transform.localPosition = new Vector3(i, j, 0);
            }
        }


    }

}
