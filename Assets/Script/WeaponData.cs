using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ���⿡ ����

[RequireComponent(typeof(PhotonView))]
public class WeaponData : MonoBehaviourPun, IPunObservable
{
    public WeaponInfo weaponInfo; //  public�� �̾��� �ν����Ϳ��� ���̴±���
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
        // �浹�� ����� Layer�� Player�̸鼭 �� �÷��̾��� PhotonView�� isMine�̶��

        PhotonView pv = other.GetComponent<PhotonView>();

        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && pv != null && pv.IsMine)
        {
            // �Ʒ� �ڵ��  transform. �� �ƴ϶� gameobject. �ص� ������.
            // �� ������ ���� ������Ʈ�� Transform�� ������ �־ 
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;

            // �浹ü ��Ȱ��ȭ�ϱ�
            photonView.RPC("SwitchEnabledCollider", RpcTarget.All, false);

            // �ڽ��� ��Ʈ��ũ �������� �浹�� �÷��̾�� �����Ѵ�.
            photonView.TransferOwnership(pv.Owner.ActorNumber);
            StartCoroutine(ResetPosition());
            // �÷��̾��� ������ ���� ��ġ�� ���� ������Ų��.(���� ���� ��ü�� �θ� �������� ����)
            // �θ� - �ڽ� ���踦 ������ ��, ���� ��ü�� Transform�� �����Ͽ� �θ� �����Ѵ�.
            // ������ �ڽ� �������� �θ�
            //  ��, ���� ��ü�� �ٸ� ��ü�� ���ӵǵ��� ����� ���� ����մϴ�.
            // ���� ��ü�� �θ�-�ڽ� ���迡�� ���ӵǴ� ����� Transform�� ���ؼ��� �̷�����ϴ�. 
            // transform.parent <= �̰� ��ü�� ���� ��ü�� �θ� ��ü�� ����Ű�� �Ӽ��Դϴ�.

            //transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;
            //transform.localPosition = Vector3.zero;
            //transform.localRotation = Quaternion.identity;
            //transform.GetComponent<BoxCollider>().enabled = false;
            //pf.myWeapon = weaponInfo;
            // pf.myWeapon = weaponInfo; // �� �ڵ� ������ 10�� �߿� 5���� ���� ��� 5���� ���� �ִ�.(weaponInfo�� ������ �ν����Ϳ�  public�̴ϱ�)

            pf.RPC_GetWeapon(weaponInfo.ammo, weaponInfo.attackPower, weaponInfo.range, (int)weaponInfo.weaponType); // RPC �Լ��� ȣ���ϵ��� ��û�Ѵ�.
            // - �ڽ��� ������ �ڽ� ������Ʈ�� ����ϰ�
            // - ���� �������� (0, 0, 0)���� �����.
            // - �ڽ��� �ڽ� ������Ʈ�� ��Ȱ��ȭ �Ѵ�.
            // - ���� ������ �÷��̾�� �����Ѵ�.

        }
    }



    [PunRPC]
    void SwitchEnabledCollider(bool onOff)
    {
        transform.GetComponent<BoxCollider>().enabled = onOff;
    }
    IEnumerator ResetPosition()
    {
        // �������� �־ isMine�� �� ������ ����Ѵ�.
        yield return new WaitUntil(() => { return photonView.IsMine; });

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DropWeapon(WeaponInfo currentInfo)
    {
        // �÷��̾��� ���Ͽ��� ���� �����.
        // - �θ�� ��ϵ� ������Ʈ�� ���� ������ ó���Ѵ�.
        // - �ڽ��� �ڽ� ������Ʈ�� Ȱ��ȭ�Ѵ�.
        // - �÷��̾��� ���� ������ �޾ƿ´�.
        transform.parent = null;
        if(weaponCoroutine == null)
        {
            weaponCoroutine = StartCoroutine(TriggerOn(3));
        }
        transform.eulerAngles = Vector3.zero; // ���� ������ ���� �ݱ� �� �ִ� �� ó�� �ڿ������� �ϱ� ���ؼ�
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
