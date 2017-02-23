using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Menu : MonoBehaviour, IPointerClickHandler {

    int menuStep; // 1 - main menu,  2- choose variant

    public GameObject startGame;
    public GameObject options;
    public GameObject eightBoardSize;
    public GameObject tenBoardSize;

    GameObject menuTopText;
    GameObject menuBottomText;
    void Start()
    {
        if (!PlayerPrefs.HasKey("BoardSize"))
        {
            PlayerPrefs.SetInt("BoardSize", 8);
        }
        goMainMenu();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float clickPosition = eventData.pressPosition.y;
        float screenSize = Screen.height;
        if(screenSize/2> clickPosition)
        {
            botMenuClick();
        }
        else
        {
            topMenuClick();
        }
    }

    void topMenuClick()
    {
        Debug.Log("u gory");

    }
    void botMenuClick()
    {
        Debug.Log("na dole");
    }

    void goMainMenu()
    {
        deleteTexts();
        menuTopText = Instantiate(startGame, gameObject.transform);
        menuTopText.transform.localPosition = new Vector3(0, -200, 0);
        menuBottomText = Instantiate(options, gameObject.transform);
        menuBottomText.transform.localPosition = new Vector3(0,-600,0);
        menuStep = 1;
    }

    void goChooseVariant()
    {
        deleteTexts();
        menuTopText = Instantiate(eightBoardSize, gameObject.transform);
        menuBottomText = Instantiate(tenBoardSize, gameObject.transform);
        menuStep = 2;
    }

    void deleteTexts()
    {
        if (menuTopText != null)
        {
            try
            {
                Destroy(menuTopText);
            }
            catch (Exception)
            {

                throw;
            }
        }
        if (menuBottomText != null)
        {
            try
            {
                Destroy(menuBottomText);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
