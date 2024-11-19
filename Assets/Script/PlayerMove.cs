using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerMove : MonoBehaviour,IPunObservable // i는 인터페이스니까 여기서 구현하고 있는 함수를 구현해야  한다.
{

    public float trackingSpeed = 3;

    CharacterController cc;
    public float moveSpeed = 3.0f;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;
    Vector3 myPrevPos;

    float mx = 0;
    float rotSpeed = 300;


    void Start()
    {
        pv = GetComponent<PhotonView>();
        myPrevPos = transform.position;
    }
    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        // 만일, 내가 소유권을 가진 캐릭터라면...
        if (pv.IsMine)
        {
            // 현재 카메라가 바라보는 방향으로 이동을 하고 싶다.
            // 이동의 조작 방식은 W,A,S,D 키를 이용한다.
            // 캐릭터 컨트롤러 클래스의 Move 함수를 이용한다.
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            Vector3 dir = new Vector3(h, 0, v);
            dir.Normalize();
            dir = transform.TransformDirection(dir);
            cc.Move(dir * moveSpeed * Time.deltaTime);

            if (myAnim != null)
            {
                myAnim.SetFloat("Horizontal", h);
                myAnim.SetFloat("Vertical", v);
            }
        }

        else
        {
            // transform.position은 현재 위치, myPos는 네트워크를 통해 수신된 목표 위치
            Vector3 targetPos = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed); 

            float dist = (targetPos - myPrevPos).magnitude;
            transform.position = dist > 0.01f ? targetPos : myPos; // myPos는 최종적으로 도달해야 하는 위치, targetPos는 보간된 중간 위치
            //  Vector2  animPos = dist > 0.01 ? Vector2.one : Vector2.zero;

            Vector3 localDir = transform.InverseTransformDirection(targetPos - myPrevPos); // 로컬 좌표로 변화하기 (먼저 보간을 계산한 뒤  로컬 좌표로 계산하기)

            float deltaX = localDir.x;
            float deltaZ = localDir.z;


            float newX = 0;
            float newZ = 0;

            if (Math.Abs(deltaX) > 0.01f)
            {
                newX = deltaX > 0 ? 1.0f : -1.0f;
            }

            if (Math.Abs(deltaZ) > 0.01f)
            {
                newZ = deltaZ > 0 ? 1.0f : -1.0f;
            }

            myPrevPos = transform.position;

            myAnim.SetFloat("Horizontal", newX); // 1.0이면 오른쪽으로 이동 -1.0이면 왼쪽으로 이동
            myAnim.SetFloat("Vertical", newZ);
        }
    }

    void Rotate()
    {
        if (pv.IsMine)
        {
            // 사용자의 마우스 좌우 드래그 입력을 받는다.
            mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

            // 입력받은 방향에 따라 플레이어를 좌우로 회전한다.
            transform.eulerAngles = new Vector3(0, mx, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime * trackingSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 매개변수는 stream은 데이터를 주고받는 길

        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            // itreable 데이터를 보낸다.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는  상태라면...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext(); // myPos가 네트워크를 통해 받은 위치로 업데이트 됨
            myRot = (Quaternion)stream.ReceiveNext();
            //Vector2 inputValue = (Vector2)stream.ReceiveNext();
            //h = inputValue.x;
            //v = inputValue.y;
        }
    }
}
