using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;

public class LobbyUIController : MonoBehaviour
{
    public GameObject panel_Login;
    public Button btn_login;
    public TMP_InputField input_nickName;
    public GameObject panel_JoinOrRoomCreate;

    public static LobbyUIController lobbyUI;
    public TMP_InputField[] roomSetting;
    public TMP_Text text_logText;

    string log;

    private void Awake()
    {
        if(lobbyUI == null)
        {
            lobbyUI = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void ShowRoomPanel()
    {
        // btn_login.interactable = true; 여기는 왜 true를 하는 지 모르겠음.
        panel_Login.gameObject.SetActive(false);
        panel_JoinOrRoomCreate.SetActive(true);
    }

    public void PrintLog(string message)
    {
        log += message + '\n';
        text_logText.text = log;
    }
}
