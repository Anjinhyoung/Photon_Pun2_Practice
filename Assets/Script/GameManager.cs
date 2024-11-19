using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    void Start()
    {
        //  코루틴을 사용하면 yield return을 통해 조건이 충족될 때까지 실행을 일시 중지할 수 있어서 코드의 흐름을 중단시키지 않고 비동기적으로 기다릴 수 있습니다.
        //  코루틴은 조건이 충족될 때까지 비동기적으로 반복 확인하며, 기다릴 수 있는 로직을 만듭니다.
        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView 에서 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SendRate = 30;
    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 randomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
    }

    void Update()
    {
        
    }
}
