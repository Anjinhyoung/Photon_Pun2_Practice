using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // 싱글톤을 사용하는 이유 특정 클래스의 인스턴스를 하나만 존재하도록 보장하는거
    public static UIManager main_ui;
    public TMP_Text[] weaponInfo;
    public Button btn_leave;

    private void Awake()
    {
        if(main_ui == null)
        {
            main_ui = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        btn_leave.onClick.AddListener(LeaveCurrentRoom);
    }

    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo[0].text = $"Ammo: {info.ammo}";
        weaponInfo[1].text = $"Damage: {info.attackPower}";
        weaponInfo[2].text = $"Weapon Type: {info.weaponType}";
    }

    public void LeaveCurrentRoom()
    {
        // 현재 방을 떠난다.
        PhotonNetwork.LeaveRoom();
        // SceneManager.LoadScene(0);
        PhotonNetwork.LoadLevel(0);
    }
}
