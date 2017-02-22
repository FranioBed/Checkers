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

    public Sprite black;
    public Sprite white;

    const int boardSize = 8;
    CheckerField [,] board;
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
        board = new CheckerField[boardSize, boardSize];
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
                Debug.Log("Right");
                curBoardPosX++;
            }
            else
            {
                Debug.Log("Left");
                curBoardPosX--;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                Debug.Log("Up");
                curBoardPosY++;
            }
            else
            {
                Debug.Log("Down");
                curBoardPosY--;
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
}
