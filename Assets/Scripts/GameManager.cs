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
    Checker [,] checkersOnBoard;
    GameObject[,] board;
    static int curBoardPosX = 0;
    static int curBoardPosY = 0;
    bool curPlayer = false;         //false for white, true for red;
    bool moveSelection = false;     //flag for move selection stage
    int selectedMoveIndex;
    List<Vector2> moveList;
    List<GameObject> moveSelectorList;

    public GameObject fullscreenLayer;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        moveSelectorList = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }
    
	void Start () {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked += fullscreenFlickedHandler;
        fullscreenLayer.GetComponent<TapGesture>().Tapped += onFullscreenTap;
        generateBoard();
        //findAllClearMoves(checkers[2, 2]);
        //findAllCaptureMoves(checkers[2, 2]);
    }

    void generateBoard()
    {
        checkersOnBoard = new Checker[boardSize, boardSize];
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

                    if (j < (boardSize/2-1))//generate red
                        createChecker(checkerObject.transform, false, new int[] { i, j });
                    if (boardSize/2 < j)//generate white
                        createChecker(checkerObject.transform, true, new int[] { i, j });


                }
                else
                {
                    board[i, j] = Instantiate(whiteField, boardObject.transform);
                }
                board[i, j].transform.SetParent(boardObject.transform);
                board[i, j].transform.localPosition = new Vector3(i, j, 0);
            }
        }
        //createChecker(checkerObject.transform,true, new int[] { 2, 2 });
        //checkers[2, 2].queen = true;
        //createChecker(checkerObject.transform, false, new int[] { 3, 3 });
        //createChecker(checkerObject.transform, false, new int[] { 5, 5 });
        //createChecker(checkerObject.transform, false, new int[] { 1, 3 });
        //createChecker(checkerObject.transform, false, new int[] { 0, 4 });
        //createChecker(checkerObject.transform, false, new int[] { 1, 1 });
        //createChecker(checkerObject.transform, false, new int[] { 3, 1 });

    }

    void createChecker(Transform transform, bool color, int[] position)
    {
        GameObject tmpObject;
        if (color)
            tmpObject = Instantiate(checkerRed, transform);
        else
            tmpObject = Instantiate(checkerWhite, transform);
        tmpObject.transform.localPosition = new Vector3(position[0], position[1], 0);
        tmpObject.transform.SetParent(transform);
        Checker checker = tmpObject.GetComponent<Checker>();
        checkersOnBoard[position[0], position[1]] = checker;
        checker.color = color;
        checker.SetPosition(position[0], position[1]);
    }


    List<Vector2> findAllCaptureMoves(Checker checker)
    {
        int[] checkerPosition = checker.GetPosition();
        List<Vector2> moveCaptureList = new List<Vector2>();
        if (checker.queen)
        {
            moveCaptureList = findAllQueenMoves(checker, true);
        }
        else
        {
           
            int side = 1;
            if (checker.color)
                side = -1;
            if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side] != null)                             //right up and destroy
                if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side].color != checker.color)
                    if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 2 * side, checkerPosition[1] + 2 * side }))
                        moveCaptureList.Add(new Vector2(checkerPosition[0] + 2 * side, checkerPosition[1] + 2 * side));

            if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side] != null)                             //left up and destroy
                if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side].color != checker.color)
                    if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side }))
                        moveCaptureList.Add(new Vector2(checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side));


            if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side] != null)                             //right down and destroy
                if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side].color != checker.color)
                    if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side }))
                        moveCaptureList.Add(new Vector2(checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side));


            if (checkersOnBoard[checkerPosition[0] - 1 * side, checkerPosition[1] - 1 * side] != null)                             //right up and destroy
                if (checkersOnBoard[checkerPosition[0] - 1 * side, checkerPosition[1] - 1 * side].color != checker.color)
                    if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] - 2 * side, checkerPosition[1] - 2 * side }))
                        moveCaptureList.Add(new Vector2(checkerPosition[0] - 2 * side, checkerPosition[1] - 2 * side));

        }


        foreach (Vector2 position in moveCaptureList)
        {
            GameObject tmpObject = Instantiate(moveSelector, gameObject.transform);
            tmpObject.transform.localPosition = new Vector3(position.x, position.y, -1);
            moveSelectorList.Add(tmpObject);
        }

        return moveCaptureList;
    }


    List<Vector2> findAllClearMoves(Checker checker)
    {
        List<Vector2> moveList = new List<Vector2>();
        if (checker.queen)
        {
            moveList = findAllQueenMoves(checker, false);
        }
        else
        {
            int[] checkerPosition = checker.GetPosition();
            int side = 1;
            if (checker.color)
                side = -1;
            if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side }))  //right up
                moveList.Add(new Vector2(checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side));

            if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] - 1 * side, checkerPosition[1] + 1 * side }))  //left up
                moveList.Add(new Vector2(checkerPosition[0] - 1 * side, checkerPosition[1] + 1 * side));
        }

        foreach (Vector2 position in moveList)
        {
            GameObject tmpObject = Instantiate(moveSelector, gameObject.transform);
            tmpObject.transform.localPosition = new Vector3(position.x, position.y, -1);
            moveSelectorList.Add(tmpObject);
        }

        return moveList;
    }

    List<Vector2> findAllQueenMoves(Checker checker, bool capture)
    {
        int[] checkerPosition = checker.GetPosition();
        List<Vector2> moveList = new List<Vector2>();
        List<Vector2> moveCaptureList = new List<Vector2>();
        
        int[] tmpPosition = new int[] { checkerPosition[0], checkerPosition[1] };
        bool blockMove = false;
        bool findEnemy = false;
        while (tmpPosition[0] < boardSize - 1 && tmpPosition[1] < boardSize - 1)  //right up
        {
            tmpPosition[0]++;
            tmpPosition[1]++;
            if (checkersOnBoard[tmpPosition[0], tmpPosition[1]] != null)
            {
                if (checkersOnBoard[tmpPosition[0], tmpPosition[1]].color == checker.color)
                    break;
                if (blockMove)
                {
                    break;
                }
                else
                {
                    blockMove = true;
                    findEnemy = true;
                }
            }
            else
            {
                if (blockMove)
                    blockMove = false;
                if (findEnemy)
                    moveCaptureList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
                else
                    moveList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
            }
        }

        tmpPosition = new int[] { checkerPosition[0], checkerPosition[1] };
        blockMove = false;
        findEnemy = false;
        while (tmpPosition[0] > 0 && tmpPosition[1] < boardSize - 1)  //left up
        {
            tmpPosition[0]--;
            tmpPosition[1]++;

            if (checkersOnBoard[tmpPosition[0], tmpPosition[1]] != null)
            {
                if (checkersOnBoard[tmpPosition[0], tmpPosition[1]].color == checker.color)
                    break;
                if (blockMove)
                {
                    break;
                }
                else
                {
                    blockMove = true;
                    findEnemy = true;
                }
            }
            else
            {
                if (blockMove)
                    blockMove = false;
                if (findEnemy)
                    moveCaptureList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
                else
                    moveList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
            }

        }

        tmpPosition = new int[] { checkerPosition[0], checkerPosition[1] };
        blockMove = false;
        findEnemy = false;
        while (tmpPosition[0] < boardSize - 1 && tmpPosition[1] > 0)  //right down
        {
            tmpPosition[0]++;
            tmpPosition[1]--;

            if (checkersOnBoard[tmpPosition[0], tmpPosition[1]] != null)
            {
                if (checkersOnBoard[tmpPosition[0], tmpPosition[1]].color == checker.color)
                    break;
                if (blockMove)
                {
                    break;
                }
                else
                {
                    blockMove = true;
                    findEnemy = true;
                }
            }
            else
            {
                if (blockMove)
                    blockMove = false;
                if (findEnemy)
                    moveCaptureList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
                else
                    moveList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
            }

        }

        tmpPosition = new int[] { checkerPosition[0], checkerPosition[1] };
        blockMove = false;
        findEnemy = false;
        while (tmpPosition[0] > 0 && tmpPosition[1] > 0)  //left down
        {
            tmpPosition[0]--;
            tmpPosition[1]--;

            if (checkersOnBoard[tmpPosition[0], tmpPosition[1]] != null)
            {
                if (checkersOnBoard[tmpPosition[0], tmpPosition[1]].color == checker.color)
                    break;
                if (blockMove)
                {
                    break;
                }
                else
                {
                    blockMove = true;
                    findEnemy = true;
                }
            }
            else
            {
                if (blockMove)
                    blockMove = false;
                if (findEnemy)
                    moveCaptureList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
                else
                    moveList.Add(new Vector2(tmpPosition[0], tmpPosition[1]));
            }

        }

        if (capture)
            return moveCaptureList;
        return moveList;
    }

    bool canMoveToPosition(int[] from, int[] destination)
    {
        if (destination[0] < 0 && destination[1] < 0 && destination[0] >= boardSize && destination[1] >= boardSize)
            return false;

        if (checkersOnBoard[destination[0], destination[1]] != null)
            return false;
        return true;
    }

    void fullscreenFlickedHandler(object sender, EventArgs e)
    {
        FlickGesture gesture = sender as FlickGesture;

        Vector2 dir = gesture.ScreenFlickVector;

        if (moveSelection)
        {
            if (dir.x > 0)
            {
                selectedMoveIndex++;
                selectedMoveIndex %= (moveList.Count + 1);
            }
            else
            {
                selectedMoveIndex--;
                selectedMoveIndex %= (moveList.Count + 1);
            }
            Debug.Log("Move: " + selectedMoveIndex);
        }
        else
        {
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
    }

    void onFullscreenTap(object sender, EventArgs e)
    {
        if (moveSelection)
        {
            if (selectedMoveIndex != moveList.Count)
            {
                int x = (int)moveList[selectedMoveIndex].x;
                int y = (int)moveList[selectedMoveIndex].y;
                checkersOnBoard[x, y] = checkersOnBoard[curBoardPosX, curBoardPosY];
                checkersOnBoard[curBoardPosX, curBoardPosY] = null;
                checkersOnBoard[x, y].SetPosition(x, y);
                curBoardPosX = x;
                curBoardPosY = y;
                fieldSelector.transform.position = new Vector3(x, y, 0);
                curPlayer = !curPlayer;
            }
            foreach (GameObject go in moveSelectorList)
            {
                Destroy(go);
            }
            moveSelectorList.Clear();
            moveSelection = false;
            moveList.Clear();
        }
        else if (checkersOnBoard[curBoardPosX,curBoardPosY] != null)
        {
            Checker cur = checkersOnBoard[curBoardPosX, curBoardPosY];
            if (cur.color == curPlayer)
            {
                moveList = findAllClearMoves(cur);
                selectedMoveIndex = moveList.Count;
                if (selectedMoveIndex > 0)
                {
                    moveSelection = true;
                }
                else
                {
                    //TODO: message for player
                    Debug.Log("No possible moves for this pawn");
                }
            }
            else
            {
                //TODO: message for player
                Debug.Log("Invalid pawn color");
            }
        }
        else
        {
            //TODO: message for player
            Debug.Log("No pawn to move");
        }
    }

    void onDestroy()
    {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked -= fullscreenFlickedHandler;
        fullscreenLayer.GetComponent<TapGesture>().Tapped -= onFullscreenTap;
    }
}
