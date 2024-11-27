using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5.0f;

    Vector3 position;

    void Start()
    {
        
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        else
        {
            transform.position = position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            // 만일, 부딪힌 대상이 플레이어 레이어를 가지고 있다면...
            if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 플레이어의 체력을 감소시킨다,(TakeDamage)
                PlayerMove pm = other.transform.GetComponent<PlayerMove>();

                if(pm != null)
                {
                    pm.RPC_TakeDamege(20, photonView.ViewID);
                }
            }

            // 자기 자신을 제거한다.
            PhotonNetwork.Destroy(gameObject);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }

        else if(stream.IsReading)
        {
            position = (Vector3)stream.ReceiveNext();
        }
    }
}
