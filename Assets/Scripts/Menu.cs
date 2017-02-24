using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour, IPointerClickHandler {

    int menuStep; // 1 - main menu,  2- choose variant

    public GameObject startGame;
    public GameObject options;
    public GameObject eightBoardSize;
    public GameObject teenBoardSize;

    GameObject menuTopText;
    GameObject menuBottomText;
    void Start()
    {
        if (!PlayerPrefs.HasKey("BoardSize"))
        {
            PlayerPrefs.SetInt("BoardSize", 8);
        }
        goMainMenu();

        TTSManager.Initialize(transform.name, "OnTTSInit");
        TTSManager.SetLanguage(TTSManager.POLISH);
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
           TTSManager.Speak("Kliknij tutaj aby sprawdzic czy ten dźwięk działa", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");

        }
    }

    void topMenuClick()
    {
        if (menuStep==1)
        {
            SceneManager.LoadScene(1);
        }
        else if(menuStep == 2)
        {

            PlayerPrefs.SetInt("BoardSize", 8);
            goMainMenu();
        }

    }
    void botMenuClick()
    {
        if (menuStep == 1)
        {
            goChooseVariant();
        }
        else if (menuStep == 2)
        {
            PlayerPrefs.SetInt("BoardSize", 10);
            goMainMenu();
        }
    }

    void goMainMenu()
    {
        TTSManager.Speak("Kliknij górną częśc ekranu by rozpocząć grę lub dolną by przejść do ustawień", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");
        Debug.Log("2");
        deleteTexts();
        menuTopText = Instantiate(startGame, gameObject.transform);
        menuTopText.transform.SetParent(gameObject.transform, true);
        menuTopText.transform.localPosition = new Vector3(0, 200, 0);
        menuBottomText = Instantiate(options, gameObject.transform);
        menuBottomText.transform.SetParent(gameObject.transform, false);
        menuBottomText.transform.localPosition = new Vector3(0,-200,0);
        menuStep = 1;
    }

    void goChooseVariant()
    {
        TTSManager.Speak("Kliknij górną częśc ekranu by ustawić tryb 8 na 8 lub dolną by ustawić tryb 10 na 10", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");
        Debug.Log("3");
        deleteTexts();
        menuTopText = Instantiate(eightBoardSize, gameObject.transform);
        menuTopText.transform.SetParent(gameObject.transform, true);
        menuTopText.transform.localPosition = new Vector3(0, 200, 0);
        menuBottomText = Instantiate(teenBoardSize, gameObject.transform);
        menuBottomText.transform.SetParent(gameObject.transform, false);
        menuBottomText.transform.localPosition = new Vector3(0, -200, 0);
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
