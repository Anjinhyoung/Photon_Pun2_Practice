using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

//   �÷��̾��� ����
public class PlayerFire : MonoBehaviourPun, IPunObservable // MonoBehaviourPun��  Photon View�� ����� �� �ִ�. �׳� PhotonView�� Rpc �Լ��� ����. 
{
    public Transform[] sockets;
    public WeaponInfo myWeapon;
    public Animator anim;

    PlayerUI playerUI;
    int weaponNumber = -1;
    void Start()
    {
        myWeapon.weaponType = WeaponType.None; // int�� �ϰ� �ʹٸ� int �� �ڷ������ٰ� ����� ��ȯ�� �� �Ŀ� �ؾ� �Ѵ�.
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            UIManager.main_ui.SetWeaponInfo(myWeapon);
        }

        playerUI = GetComponentInChildren<PlayerUI>();

        // ������ �÷��̾��� �г��Ӱ� �÷��� �Է��Ѵ�. (��: ���, ����: ����)
        Color nameColor = photonView.IsMine ? new Color(0, 1, 0) : new Color(1, 0, 0);
        playerUI.SetNickName(photonView.Owner.NickName, nameColor);

        // ������ �÷��̾��� ü�� ���� ������ �Է��Ѵ�. (��: ���, ����: ��Ȳ��)
        Color healthColor = photonView.IsMine ? new Color(0, 1, 0) : new Color(1, 0.2f, 0);
        playerUI.SetColor(healthColor);
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0) && photonView.IsMine && myWeapon.weaponType != WeaponType.None)
        {
            Fire();
        }

        if(photonView.IsMine && Input.GetKeyDown(KeyCode.F) && myWeapon.weaponType != WeaponType.None)
        {
            RPC_DropWeapon();
        }

    }

    void Fire()
    {
        // ī�޶��� �߾��� �������� �������� ����ĳ��Ʈ�� �Ѵ�.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        bool result = Physics.Raycast(ray, out hitInfo, myWeapon.range, ~(1 << 6));
        if (result)
        {
            // ����   ���� ������Ʈ�� �±װ� "Enemy"���
            if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // ������ ó���� �Ѵ�.
                hitInfo.transform.GetComponent<PlayerMove>().RPC_TakeDamege(myWeapon.attackPower, photonView.ViewID);
            }

            // �׷��� �ʴٸ� ���� ��ġ�� ���� ����Ʈ�� ����Ѵ�.

            else
            {

            }
        }

        // �Ѿ��� ������ 1 ���ҽ�Ű�� (��, 0���ϰ� ���� �ʵ��� �Ѵ�.)
        myWeapon.ammo = Mathf.Max(--myWeapon.ammo, 0);
        UIManager.main_ui.SetWeaponInfo(myWeapon);
    }



    void RPC_DropWeapon()
    {
        // ���� ���⿡ �ִ� WeaponData ������Ʈ�� DropWeapon �Լ��� �����Ѵ�.
        WeaponData data = sockets[(int)myWeapon.weaponType].GetChild(0).GetComponent<WeaponData>(); // public�̶� �����ϴ�.

        if (data != null) // Ŭ������ null �� �� �ֱ���
        {
            data.DropWeapon(myWeapon);
        }

        photonView.RPC("DropMyWeapon", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DropMyWeapon()
    {
        // ���� ����(myWeapon ����)�� �ʱ�ȭ �Ѵ�.
        if (photonView.IsMine)
        {
            myWeapon = new WeaponInfo();
            UIManager.main_ui.SetWeaponInfo(myWeapon);
        }

        anim.SetBool("GetPistol", false);
        anim.SetBool("GetRifle", false);    
    }


    public void RPC_GetWeapon(int ammo, float attackPower, float range, int weaponType)
    {
        photonView.RPC("GetWeapon", RpcTarget.AllBuffered, ammo, attackPower, range, weaponType);
    }


    [PunRPC]
    public void GetWeapon(int ammo, float attackPower, float range, int weaponType)
    {
        myWeapon.SetInformation(ammo, attackPower, range, (WeaponType)weaponType);

        if (photonView.IsMine)
        {
            UIManager.main_ui.SetWeaponInfo(myWeapon);

            if (myWeapon.weaponType == WeaponType.PistolType)
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

        else
        {
            if (weaponType == 0)
            {
                anim.SetBool("GetPistol", true);
                anim.SetBool("GetRifle", false);
            }

            else if (weaponType == 1)
            {
                anim.SetBool("GetRifle", true);
                anim.SetBool("GetPistol", false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)myWeapon.weaponType);
        }

        else if (stream.IsReading)
        {
            weaponNumber = (int)stream.ReceiveNext();
        }
    }
}
