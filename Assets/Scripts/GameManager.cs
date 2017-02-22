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

    public GameObject fieldSelector;
    public GameObject moveSelector;
    public GameObject blackField;
    public GameObject whiteField;
    public GameObject checkerRed;
    public GameObject checkerWhite;

    const int boardSize = 8;
    Checker [,] checkers;
    GameObject[,] board;
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
    
	void Start () {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked += fullscreenFlickedHandler;
        generateBoard();
    }

    void generateBoard()
    {
        checkers = new Checker[boardSize, boardSize];
        board = new GameObject[boardSize, boardSize];
        GameObject boardObject = Instantiate(new GameObject("Board"), gameObject.transform);
        boardObject.transform.localPosition = new Vector3(0, 0, 0.1f);
        GameObject checkerObject = Instantiate(new GameObject("Checkers"), gameObject.transform);
        checkerObject.transform.localPosition = new Vector3(0, 0, 0);

        bool color = true;  //true means black, false white 
        for (int i = 0; i < boardSize; i++)
        {
            color = !color;
            for (int j = 0; j < boardSize; j++)
            {
                color = !color;
                if (color)
                {
                    board[i, j] = Instantiate(blackField, boardObject.transform);
                    if (j < 3)//generate red
                    {
                        GameObject tmpObject = Instantiate(checkerRed, checkerObject.transform);
                        tmpObject.transform.localPosition = new Vector3(i, j, 0);
                        tmpObject.transform.SetParent(checkerObject.transform);
                        checkers[i, j] = tmpObject.GetComponent<Checker>();
                        checkers[i, j].color = true;
                    }
                    if (boardSize - 4 < j)//generate white
                    {
                        GameObject tmpObject = Instantiate(checkerWhite, checkerObject.transform);
                        tmpObject.transform.localPosition = new Vector3(i, j, 0);
                        tmpObject.transform.SetParent(checkerObject.transform);
                        checkers[i, j] = tmpObject.GetComponent<Checker>();
                        checkers[i, j].color = false;
                    }
                }
                else
                {
                    board[i, j] = Instantiate(whiteField, boardObject.transform);
                }
                board[i, j].transform.SetParent(boardObject.transform);
                board[i, j].transform.localPosition = new Vector3(i, j, 0);
            }
        }
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
        Vector3 selectorPosOnScreen = board[curBoardPosX, curBoardPosY].transform.position;
        fieldSelector.transform.position = new Vector3(selectorPosOnScreen.x, selectorPosOnScreen.y, 0.05f);
        Debug.Log(curBoardPosX + "," + curBoardPosY);
    }

    void onDestroy()
    {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked -= fullscreenFlickedHandler;
    }
}
