using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomPanel : MonoBehaviour
{
    public TMP_Text[] room_Text = new TMP_Text[3];
    public Button btn_join;

    public void SetRoomInfo(RoomInfo room)
    {
        room_Text[0].text = room.Name;
        room_Text[1].text = $"({room.PlayerCount} / {room.MaxPlayers})";
        room_Text[2].text = "JinHyoung";
    }
}
