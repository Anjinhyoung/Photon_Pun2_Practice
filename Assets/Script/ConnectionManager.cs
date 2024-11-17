using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
// using UnityEditor.Timeline;
using System;

public class ConnectionManager : MonoBehaviourPunCallbacks
{

    void Start()
    {
        // Unity���� ȭ�� �ػ󵵿� ��ü ȭ�� ��带 �����ϴ� �Լ�
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
    }

    void Update()
    {
        
    }

    public void StartLogin()
    {
        // ������ ���� ����
        if(LobbyUIController.lobbyUI.input_nickName.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyUIController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = true; // �ڵ����� ��ũ�� �����(������ Scene�� ����...)

            // ���� ������ ���� ��û  (���� �������� ������ ������ �˾Ƽ� ���ش�.) 
            PhotonNetwork.ConnectUsingSettings();
            LobbyUIController.lobbyUI.btn_login.interactable = true; 
        }
    }
    public override void OnConnected()
    {
        base.OnConnected();

        // ���� ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // ���� ������ ����Ѵ�.
        Debug.LogError("Disconntected from Server" + cause); // ���� ����, ������ ���� ������ �� �߿� �� �Ǹ� �˷��ִ� �Լ� 
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // ������ ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // ������ �κ�� ����.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // ���� �κ� ������ �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.ShowRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        if(roomName.Length > 0 && playerCount > 1)
        {
            // ���� ���� �����.(������ ��)
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true; // ��������  �� ����  �˻��� �� �ְ� �ϴ���

            PhotonNetwork.CreateRoom(roomName , roomOpt, TypedLobby.Default); // ���� ����� �Լ�
        }
    }

    public void JoinRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;

        if(roomName.Length > 0)
        {
            PhotonNetwork.JoinRoom(roomName); // ������� �濡 ���� �Լ�
        }
    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // ���������� ���� �����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("�� �������");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // ���������� �濡 ����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("�濡 ���� ����");
  
        // �濡 ������ ģ������ ��� 1�� ������ �̵�����!
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // �뿡 ������ ������ ������ ����Ѵ�.
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("�濡 ����" + message);
    }


    // �뿡 �ٸ� �÷��̾ �������� ���� �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName} ���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // �뿡 �ٸ� �÷��̾ �������� ���� �ݹ� �Լ�

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName} ���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

}
