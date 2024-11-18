using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using System;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public Transform scrollContent;
    public GameObject[] panelList;

    List<RoomInfo> cachedRoomList = new List<RoomInfo>();
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
        // Join  관련 패널을 활성화한다.
        ChangePanel(1, 2);
    }

    /// <summary>
    /// 패널의 패턴 변경
    /// </summary>
    /// <param name="offIndex">꺼야될 패널 인덱스</param>
    /// <param name="onIndex">켜야될 패널 인덱스</param>
    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
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

    // 현재 로비에서 룸의 변경사항을 알려주는 콜백 함수(진짜 딱 변경된 것만 알려줘)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach(RoomInfo room in roomList)
        {
            // 만일, 갱신된 룸 정보가 제거 리스트에 있다면...
            if (room.RemovedFromList)
            {
                // cachedRoomList에서 해당 룸을 제거한다.
                cachedRoomList.Remove(room);
            }
            // 그렇지 않다면...
            else
            {
                // 만일, 이미 cachedRoomList에 있는 방이라면...
                if (cachedRoomList.Contains(room))
                {
                    // 기존 룸 정보를 제거한다.
                    cachedRoomList.Remove(room);
                }
                // 새 룸을 cachedRoomList에 추가한다.
                cachedRoomList.Add(room);
            }

        }

        // 기존의 모든 방 정보를 삭제한다. 
        for(int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach(RoomInfo room in cachedRoomList)
        {
            // cachedRoomList에 있는 모든 방을 만들어서 스크롤뷰에 추가한다.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            // go.GetComponent<RoomPanel>().SetRoomInfo(room);     //  이렇게 하면 변수가 필요없다. 당연하지 그냥 함수만 필요한 건데 함수를 변수처럼 하는 델리게이트가 꼭 필요 없잖아
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            // 버튼에 방 입장 기능 연결하기
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

}
