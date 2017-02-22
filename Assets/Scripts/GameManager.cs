using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

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

    const int boardSize = 8;

    CheckerField [,] board;
    static int curBoardPosX, curBoardPosY;

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

    }

    void onDestroy()
    {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked -= fullscreenFlickedHandler;
    }
}
