using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//   플레이어의 부착
public class PlayerFire : MonoBehaviour
{
    public Transform[] sockets;
    public WeaponInfo myWeapon;
    public Animator anim;

    void Start()
    {
        myWeapon.weaponType = WeaponType.None; // int로 하고 싶다면 int 형 자료형에다가 명시적 변환을 한 후에 해야 한다.
        UIManager.main_ui.SetWeaponInfo(myWeapon);
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0) && myWeapon.weaponType != WeaponType.None)
        {
            Fire();
        }

        if(Input.GetKeyDown(KeyCode.F) && myWeapon.weaponType != WeaponType.None)
        {
            DropMyWeapon();
        }

    }

    void Fire()
    {
        // 카메라의 중앙을 기준으로 전방으로 레이캐스트를 한다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        bool result = Physics.Raycast(ray, out hitInfo, myWeapon.range, ~(1 << 6));
        if (result)
        {
            // 만일   닿은 오브젝트의 태그가 "Enemy"라면
            if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 데미지 처리를 한다.

            }

            // 그렇지 않다면 닿은 위치에 파편 이펙트를 출력한다.

            else
            {

            }
        }

        // 총알의 갯수를 1 감소시키고 (단, 0이하가 되지 않도록 한다.)
        myWeapon.ammo = Mathf.Max(--myWeapon.ammo, 0);
        UIManager.main_ui.SetWeaponInfo(myWeapon);
    }


    
    void DropMyWeapon()
    {
        // 나의 무기에 있는 WeaponData 컴포넌트의 DropWeapon 함수를 실행한다.
       
        WeaponData data = sockets[(int)myWeapon.weaponType].GetChild(0).GetComponent<WeaponData>(); // public이라서 가능하다.

        if (data != null) // 클래스가 null 될 수 있구나
        {
            data.DropWeapon(myWeapon);

            // 무기 상태(myWeapon 변수)를 초기화 한다.
            myWeapon = new WeaponInfo();
            UIManager.main_ui.SetWeaponInfo(myWeapon);

            if (myWeapon.weaponType == WeaponType.None)
            {
                anim.SetBool("GetPistol", false);
                anim.SetBool("GetRifle", false);
            }
        }
    }

    public void GetWeapon()
    {
        UIManager.main_ui.SetWeaponInfo(myWeapon);
        if(myWeapon.weaponType == WeaponType.PistolType)
        {
            anim.SetBool("GetPistol", true);
            anim.SetBool("GetRifle", false);
        }

        else if(myWeapon.weaponType == WeaponType.RifleType)
        {
            anim.SetBool("GetRifle", true);
            anim.SetBool("GetPistol", false);
        }
    }
}
