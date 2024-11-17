using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // 싱글톤을 사용하는 이유 특정 클래스의 인스턴스를 하나만 존재하도록 보장하는거
    public static UIManager main_ui;
    public TMP_Text[] weaponInfo;
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo[0].text = $"Ammo: {info.ammo}";
        weaponInfo[1].text = $"Damage: {info.attackPower}";
        weaponInfo[2].text = $"Weapon Type: {info.weaponType}";
    }
}
