using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerMove : MonoBehaviour,IPunObservable // i�� �������̽��ϱ� ���⼭ �����ϰ� �ִ� �Լ��� �����ؾ�  �Ѵ�.
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
        // ����, ���� �������� ���� ĳ���Ͷ��...
        if (pv.IsMine)
        {
            // ���� ī�޶� �ٶ󺸴� �������� �̵��� �ϰ� �ʹ�.
            // �̵��� ���� ����� W,A,S,D Ű�� �̿��Ѵ�.
            // ĳ���� ��Ʈ�ѷ� Ŭ������ Move �Լ��� �̿��Ѵ�.
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
            // transform.position�� ���� ��ġ, myPos�� ��Ʈ��ũ�� ���� ���ŵ� ��ǥ ��ġ
            Vector3 targetPos = Vector3.Lerp(transform.position, myPos, Time.deltaTime * trackingSpeed); 

            float dist = (targetPos - myPrevPos).magnitude;
            transform.position = dist > 0.01f ? targetPos : myPos; // myPos�� ���������� �����ؾ� �ϴ� ��ġ, targetPos�� ������ �߰� ��ġ
            //  Vector2  animPos = dist > 0.01 ? Vector2.one : Vector2.zero;

            Vector3 localDir = transform.InverseTransformDirection(targetPos - myPrevPos); // ���� ��ǥ�� ��ȭ�ϱ� (���� ������ ����� ��  ���� ��ǥ�� ����ϱ�)

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

            myAnim.SetFloat("Horizontal", newX); // 1.0�̸� ���������� �̵� -1.0�̸� �������� �̵�
            myAnim.SetFloat("Vertical", newZ);
        }
    }

    void Rotate()
    {
        if (pv.IsMine)
        {
            // ������� ���콺 �¿� �巡�� �Է��� �޴´�.
            mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

            // �Է¹��� ���⿡ ���� �÷��̾ �¿�� ȸ���Ѵ�.
            transform.eulerAngles = new Vector3(0, mx, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime * trackingSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // �Ű������� stream�� �����͸� �ְ�޴� ��

        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��...
        if (stream.IsWriting)
        {
            // itreable �����͸� ������.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о����  ���¶��...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext(); // myPos�� ��Ʈ��ũ�� ���� ���� ��ġ�� ������Ʈ ��
            myRot = (Quaternion)stream.ReceiveNext();
            //Vector2 inputValue = (Vector2)stream.ReceiveNext();
            //h = inputValue.x;
            //v = inputValue.y;
        }
    }
}
