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
        // Unity에서 화면 해상도와 전체 화면 모드를 설정하는 함수
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
    }

    void Update()
    {
        
    }

    public void StartLogin()
    {
        // 접속을 위한 설정
        if(LobbyUIController.lobbyUI.input_nickName.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyUIController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = true; // 자동으로 싱크를 맞춘다(방장의 Scene을 기준...)

            // 네임 서버에 관한 요청  (네임 서버에서 마스터 서버는 알아서 해준다.) 
            PhotonNetwork.ConnectUsingSettings();
            LobbyUIController.lobbyUI.btn_login.interactable = true; 
        }
    }
    public override void OnConnected()
    {
        base.OnConnected();

        // 네임 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // 실패 원인을 출력한다.
        Debug.LogError("Disconntected from Server" + cause); // 네임 서버, 마스터 서버 접속이 둘 중에 안 되면 알려주는 함수 
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // 마스터 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // 서버의 로비로 들어간다.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.ShowRoomPanel();
    }

    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        if(roomName.Length > 0 && playerCount > 1)
        {
            // 나의 룸을 만든다.(서버의 룸)
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true; // 누군가가  내 방을  검색할 수 있게 하느냐

            PhotonNetwork.CreateRoom(roomName , roomOpt, TypedLobby.Default); // 방을 만드는 함수
        }
    }

    public void JoinRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;

        if(roomName.Length > 0)
        {
            PhotonNetwork.JoinRoom(roomName); // 만들어진 방에 들어가는 함수
        }
    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("방 만들어짐");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 성공적으로 방에 입장되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.PrintLog("방에 입장 성공");
  
        // 방에 입장한 친구들은 모두 1번 씬으로 이동하자!
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 룸에 입장이 실패한 이유를 출력한다.
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("방에 실패" + message);
    }


    // 룸에 다른 플레이어가 입장했을 때의 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName} 님이 입장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    // 룸에 다른 플레이어가 퇴장했을 때의 콜백 함수

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName} 님이 퇴장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

}
