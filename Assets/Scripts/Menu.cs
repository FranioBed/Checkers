using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour, IPointerClickHandler {

    int menuStep; // 1 - main menu,  2- choose variant
    
    public Text menuTopText;
    public Text menuBottomText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TTSManager.Speak("Do widzenia", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");
            Application.Quit();
        }
    }

    void Start()
    {
        
        if (!PlayerPrefs.HasKey("BoardSize"))
        {
            PlayerPrefs.SetInt("BoardSize", 8);
        }
        TTSManager.Initialize(transform.name, "OnTTSInit");
        DateTime time1 = System.DateTime.Now;

        while (!TTSManager.IsInitialized() && (System.DateTime.Now - time1).TotalSeconds<2)
        {
        }
        TTSManager.SetLanguage(TTSManager.POLISH);
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
        if (menuStep==1)
        {
            TTSManager.Stop();
            SceneManager.LoadScene(1);
        }
        else if(menuStep == 2)
        {

            PlayerPrefs.SetInt("BoardSize", 8);

            TTSManager.Speak("Ustawiono tryb osiem na osiem", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");

            while (TTSManager.IsSpeaking()) ;
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

            TTSManager.Speak("Ustawiono tryb dziesięć na dziesięć", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");

            while (TTSManager.IsSpeaking()) ;


            goMainMenu();
        }
    }

    void goMainMenu()
    {
        if (TTSManager.IsInitialized())
        {
            TTSManager.Speak("Menu główne. Dotknij górną częśc ekranu by rozpocząć grę lub dolną by przejść do ustawień", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");
        }
        menuTopText.text = "START";
        menuTopText.fontSize = 100;
        menuBottomText.text = "USTAWIENIA";
        menuBottomText.fontSize = 60;
        menuStep = 1;
    }

    void goChooseVariant()
    {
        if (TTSManager.IsInitialized())
        {
            TTSManager.Speak("Ustawienia. Dotknij górną częśc ekranu by ustawić tryb osiem na osiem lub dolną by ustawić tryb dziesięć na dziesięć", false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_0");
        }
        menuTopText.text = "8 x 8";
        menuTopText.fontSize = 120;
        menuBottomText.text = "10 x 10";
        menuBottomText.fontSize = 120;
        menuStep = 2;
    }

  
}
