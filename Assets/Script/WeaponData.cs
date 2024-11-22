using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 무기에 부착

[RequireComponent(typeof(PhotonView))]
public class WeaponData : MonoBehaviourPun, IPunObservable
{
    public WeaponInfo weaponInfo; //  public이 이어져 인스펙터에도 보이는구나
    Coroutine weaponCoroutine;

    Vector3 syncPos;
    Quaternion syncRot;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = syncPos;
            transform.rotation = syncRot;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상의 Layer가 Player이면서 그 플레이어의 PhotonView가 isMine이라면

        PhotonView pv = other.GetComponent<PhotonView>();

        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && pv != null && pv.IsMine)
        {
            // 아래 코드는  transform. 이 아니라 gameobject. 해도 괜찮다.
            // 그 이유는 게임 오브젝트에 Transform을 가지고 있어서 
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;

            // 충돌체 비활성화하기
            photonView.RPC("SwitchEnabledCollider", RpcTarget.All, false);

            // 자신의 네트워크 소유권을 충돌한 플레이어에게 이전한다.
            photonView.TransferOwnership(pv.Owner.ActorNumber);
            StartCoroutine(ResetPosition());
            // 플레이어의 지정된 소켓 위치에 총을 부착시킨다.(현재 게임 객체의 부모를 소켓으로 설정)
            // 부모 - 자식 관계를 설정할 때, 게임 객체의 Transform을 참조하여 부모를 설정한다.
            // 왼쪽이 자식 오른쪽이 부모
            //  즉, 현재 객체가 다른 객체에 종속되도록 만들기 위해 사용합니다.
            // 게임 객체가 부모-자식 관계에서 종속되는 방식은 Transform을 통해서만 이루어집니다. 
            // transform.parent <= 이거 자체가 현재 객체의 부모 객체를 가리키는 속성입니다.

            //transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;
            //transform.localPosition = Vector3.zero;
            //transform.localRotation = Quaternion.identity;
            //transform.GetComponent<BoxCollider>().enabled = false;
            //pf.myWeapon = weaponInfo;
            // pf.myWeapon = weaponInfo; // 이 코드 때문에 10발 중에 5발을 쏴도 계속 5발이 남아 있다.(weaponInfo에 정보는 인스펙터에  public이니까)

            pf.RPC_GetWeapon(weaponInfo.ammo, weaponInfo.attackPower, weaponInfo.range, (int)weaponInfo.weaponType); // RPC 함수를 호출하도록 요청한다.
            // - 자신을 소켓의 자식 오브젝트로 등록하고
            // - 로컬 포지션을 (0, 0, 0)으로 맞춘다.
            // - 자신의 박스 컴포넌트를 비활성화 한다.
            // - 무기 정보를 플레이어에게 전달한다.

        }
    }



    [PunRPC]
    void SwitchEnabledCollider(bool onOff)
    {
        transform.GetComponent<BoxCollider>().enabled = onOff;
    }
    IEnumerator ResetPosition()
    {
        // 소유권이 있어서 isMine이 될 때까지 대기한다.
        yield return new WaitUntil(() => { return photonView.IsMine; });

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DropWeapon(WeaponInfo currentInfo)
    {
        // 플레이어의 소켓에서 총을 때어난다.
        // - 부모로 등록된 오브젝트를 없는 것으로 처리한다.
        // - 자신의 박스 컴포넌트를 활성화한다.
        // - 플레이어의 무기 정보를 받아온다.
        transform.parent = null;
        if(weaponCoroutine == null)
        {
            weaponCoroutine = StartCoroutine(TriggerOn(3));
        }
        transform.eulerAngles = Vector3.zero; // 총을 놓으면 원래 줍기 전 있던 것 처럼 자연스럽게 하기 위해서
        weaponInfo = currentInfo;
    }

    IEnumerator TriggerOn(float time)
    {
        yield return new WaitForSeconds(time);
        photonView.RPC("SwitchEnabledCollider", RpcTarget.All, true);
        weaponCoroutine = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }

        else if (stream.IsReading)
        {
            syncPos = (Vector3)stream.ReceiveNext();
            syncRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
