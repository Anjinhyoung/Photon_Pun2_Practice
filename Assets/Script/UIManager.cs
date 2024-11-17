using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // �̱����� ����ϴ� ���� Ư�� Ŭ������ �ν��Ͻ��� �ϳ��� �����ϵ��� �����ϴ°�
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
