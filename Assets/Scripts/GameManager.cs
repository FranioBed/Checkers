using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.SceneManagement;

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

    public int boardSize = 8;
    Checker [,] checkersOnBoard;
    GameObject[,] board;
    static int curBoardPosX = 0;
    static int curBoardPosY = 0;
    bool curPlayer = false;         //false for white, true for red;
    bool moveSelection = false;     //flag for move selection stage
    int selectedMoveIndex;
    List<Vector2> moveList;
    List<Checker> checkersToMove;
    List<GameObject> moveSelectorList;
    List<Checker> takenList;

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
        checkersToMove = new List<Checker>();
        takenList = new List<Checker>();
    }
    
	void Start () {
        fullscreenLayer.GetComponent<FlickGesture>().Flicked += fullscreenFlickedHandler;
        fullscreenLayer.GetComponent<TapGesture>().Tapped += onFullscreenTap;
        generateBoard();
        getAllMoves();
        //findAllClearMoves(checkers[2, 2]);
        //findAllCaptureMoves(checkers[2, 2]);
    }

    void generateBoard()
    {
        if (PlayerPrefs.HasKey("BoardSize"))
            boardSize = PlayerPrefs.GetInt("BoardSize");
        boardSize = PlayerPrefs.GetInt("BoardSize");
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
            if (checkerPosition[0] + 2 * side < boardSize && checkerPosition[1] + 2 * side  < boardSize && checkerPosition[0] + 2 * side >=0 && checkerPosition[1] + 2 * side >= 0)
                if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side] != null)                             //right up and destroy
                    if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] + 1 * side].color != checker.color)
                        if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 2 * side, checkerPosition[1] + 2 * side }))
                            moveCaptureList.Add(new Vector2(checkerPosition[0] + 2 * side, checkerPosition[1] + 2 * side));

            if (checkerPosition[0] - 2 * side < boardSize && checkerPosition[1] + 2 * side < boardSize && checkerPosition[0] - 2 * side >= 0 && checkerPosition[1] + 2 * side >= 0)
                if (checkersOnBoard[checkerPosition[0] - 1 * side, checkerPosition[1] + 1 * side] != null)                             //left up and destroy
                    if (checkersOnBoard[checkerPosition[0] - 1 * side, checkerPosition[1] + 1 * side].color != checker.color)
                        if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] - 2 * side, checkerPosition[1] + 2 * side }))
                            moveCaptureList.Add(new Vector2(checkerPosition[0] - 2 * side, checkerPosition[1] + 2 * side));


            if (checkerPosition[0] + 2 * side < boardSize && checkerPosition[1] - 2 * side < boardSize && checkerPosition[0] + 2 * side >= 0 && checkerPosition[1] - 2 * side >= 0)
                if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side] != null)                             //right down and destroy
                    if (checkersOnBoard[checkerPosition[0] + 1 * side, checkerPosition[1] - 1 * side].color != checker.color)
                        if (canMoveToPosition(checkerPosition, new int[] { checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side }))
                            moveCaptureList.Add(new Vector2(checkerPosition[0] + 2 * side, checkerPosition[1] - 2 * side));

            
            if (checkerPosition[0] - 2 * side < boardSize && checkerPosition[1] - 2 * side < boardSize && checkerPosition[0] - 2 * side >= 0 && checkerPosition[1] - 2 * side >= 0)
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
        if (destination[0] < 0 || destination[1] < 0 || destination[0] >= boardSize || destination[1] >= boardSize)
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
            int mod = moveList.Count + 1;
            if (takenList.Count > 0)
            {
                mod--;
            }
            if (dir.x > 0)
            {
                selectedMoveIndex++;
                selectedMoveIndex %= mod;
            }
            else
            {
                selectedMoveIndex--;
                if (selectedMoveIndex < 0)
                {
                    selectedMoveIndex = mod - 1;
                }
                selectedMoveIndex %= mod;
            }
            Debug.Log("Move: " + selectedMoveIndex);
            if (selectedMoveIndex == moveList.Count)
            {
                fieldSelector.transform.position = new Vector3(curBoardPosX, curBoardPosY, -2f);
                FindObjectOfType<Camera>().transform.position = new Vector3(curBoardPosX, curBoardPosY, -10);
            }
            else
            {
                fieldSelector.transform.position = new Vector3(moveList[selectedMoveIndex].x, moveList[selectedMoveIndex].y, -2f);
                FindObjectOfType<Camera>().transform.position = new Vector3(moveList[selectedMoveIndex].x, moveList[selectedMoveIndex].y, -10);
            }
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

            curBoardPosX = curBoardPosX.Clamp(0, boardSize-1);
            curBoardPosY = curBoardPosY.Clamp(0, boardSize-1);
            Vector3 selectorPosOnScreen = board[curBoardPosX, curBoardPosY].transform.position;
            fieldSelector.transform.position = new Vector3(selectorPosOnScreen.x, selectorPosOnScreen.y, 0.05f);
            FindObjectOfType<Camera>().transform.position = new Vector3(selectorPosOnScreen.x, selectorPosOnScreen.y, -10);
            Debug.Log(curBoardPosX + "," + curBoardPosY);
        }
    }

    void getAllMoves()
    {
        checkersToMove.Clear();
        foreach (Checker checker in checkersOnBoard)
        {
            if (checker != null && checker.color == curPlayer && findAllCaptureMoves(checker).Count > 0)
            {
                checkersToMove.Add(checker);
            }
        }
        if (checkersToMove.Count == 0)
        {
            foreach (Checker checker in checkersOnBoard)
            {
                if (checker != null && checker.color == curPlayer && findAllClearMoves(checker).Count > 0)
                {
                    checkersToMove.Add(checker);
                }
            }
        }
        foreach (GameObject go in moveSelectorList)
        {
            Destroy(go);
        }
        moveSelectorList.Clear();

        Debug.Log("Possible moves: " + checkersToMove.Count.ToString());
        if (checkersToMove.Count == 0)
        {
            //currentPlayerLost
            SceneManager.LoadScene(0);
        }
    }

    void onFullscreenTap(object sender, EventArgs e)
    {
        if (moveSelection)
        {
            bool pawnTaken = false;
            if (selectedMoveIndex != moveList.Count)
            {
                pawnTaken = false;
                int x = (int)moveList[selectedMoveIndex].x;
                int y = (int)moveList[selectedMoveIndex].y;
                //Debug.Log("Destination before move: " + checkersOnBoard[x, y]);
                checkersOnBoard[x, y] = checkersOnBoard[curBoardPosX, curBoardPosY];
                checkersOnBoard[curBoardPosX, curBoardPosY] = null;
                checkersOnBoard[x, y].SetPosition(x, y);
                //Debug.Log("origin after move: " + checkersOnBoard[curBoardPosX, curBoardPosY]);
                //Debug.Log("Destination after move: " + checkersOnBoard[x, y]);

                int moveX = x - curBoardPosX;
                int moveY = y - curBoardPosY;
                if (Math.Abs(moveX) > 1)
                {
                    int xi, yi;
                    

                    for (int i = 1; i < Math.Abs(moveX); i++)
                    {
                        if (moveX > 0)
                        {
                            xi = curBoardPosX + i;
                        }
                        else
                        {
                            xi = curBoardPosX - i;
                        }
                        if (moveY > 0)
                        {
                            yi = curBoardPosY + i;
                        }
                        else
                        {
                            yi = curBoardPosY - i;
                        }

                        if (checkersOnBoard[xi, yi] != null)
                        {
                            takenList.Add(checkersOnBoard[xi, yi]);
                            checkersOnBoard[xi, yi] = null;
                            pawnTaken = true;
                        }
                    }
                }

                curBoardPosX = x;
                curBoardPosY = y;
                //Debug.Log("Destination after move and logic: " + checkersOnBoard[curBoardPosX, curBoardPosY]);

                if (!pawnTaken)
                {
                    curPlayer = !curPlayer;
                }
            }

            foreach (GameObject go in moveSelectorList)
            {
                Destroy(go);
            }

            moveSelectorList.Clear();
            moveList.Clear();
            if (takenList.Count > 0)
            {
                //Debug.Log("origin before finding next capture: " + checkersOnBoard[curBoardPosX, curBoardPosY]);
                moveList = findAllCaptureMoves(checkersOnBoard[curBoardPosX, curBoardPosY]);
            }
            if (moveList.Count == 0)
            {
                foreach(Checker taken in takenList)
                {
                    Destroy(taken.gameObject);
                }
                takenList.Clear();
                if (pawnTaken)
                {
                    curPlayer = !curPlayer;
                }
                moveSelection = false;
                if ((checkersOnBoard[curBoardPosX,curBoardPosY].color==false && curBoardPosY == 7)||(checkersOnBoard[curBoardPosX, curBoardPosY].color == true && curBoardPosY == 0))
                {
                    checkersOnBoard[curBoardPosX, curBoardPosY].queen = true;
                }
                getAllMoves();
            }
        }
        else if (checkersOnBoard[curBoardPosX,curBoardPosY] != null)
        {
            Checker cur = checkersOnBoard[curBoardPosX, curBoardPosY];
            if (checkersToMove.Contains(cur))
            {
                moveList = findAllCaptureMoves(cur);
                if (moveList.Count == 0)
                {
                    moveList = findAllClearMoves(cur);
                }
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
                Debug.Log("No possible move");
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
