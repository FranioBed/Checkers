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

    int boardSize;
    Checker [,] board;
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

    void generateBoard(int boardSize)
    {
        this.boardSize = boardSize;
        board = new Checker[boardSize, boardSize];
        boardField = new GameObject[boardSize, boardSize];

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {

            }
        }
    }

}
