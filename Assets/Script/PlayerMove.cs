using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using Photon.Voice.PUN;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerMove : PlayerState, IPunObservable , IInteractionInterface// i�� �������̽��ϱ� ���⼭ �����ϰ� �ִ� �Լ��� �����ؾ�  �Ѵ�. 
{

    public float trackingSpeed = 3;
    PlayerUI healthUI;
    public Vector3 shakePower;
    public RawImage voiceIcon;

    CharacterController cc;
    // public float moveSpeed = 3.0f;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;
    Vector3 myPrevPos;
    bool isShaking = false;

    PhotonVoiceView voiceView;
    bool isTalking = false;
    float hpSync = 0;
    bool requestLoadLevel = false;

    float mx = 0;
    // float rotSpeed = 300;


    void Start()
    {
        pv = GetComponent<PhotonView>();
        myPrevPos = transform.position;
        voiceView = GetComponent<PhotonVoiceView>();

        // ���� ü���� �ʱ�ȭ�Ѵ�.
        currentHealth = maxHealth;

        // ���̾ �����Ѵ�.
        gameObject.layer = pv.IsMine ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");

        playerState = PlayerState_.RUN;
    }
    void Update()
    {
        if(playerState == PlayerState_.RUN && !EventSystem.current.currentSelectedGameObject == null)
        {
            Move();
            Rotate();
        }

        if (pv.IsMine)
        {
            // ���� ���� �ϰ� �ִٸ� ���̽� �������� Ȱ��ȭ�Ѵ�.
            voiceIcon.transform.gameObject.SetActive(voiceView.IsRecording);
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);

            // ���� ü���� �����Ѵ�.
            if(currentHealth != hpSync)
            {
                currentHealth = hpSync;
                healthUI.SetHpValue(currentHealth, maxHealth);
            }
        }
    
        if(!requestLoadLevel && SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(GoMainScene());
        }
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
            stream.SendNext(voiceView.IsRecording);
            stream.SendNext(currentHealth);
        }
        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о����  ���¶��...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext(); // myPos�� ��Ʈ��ũ�� ���� ���� ��ġ�� ������Ʈ ��
            myRot = (Quaternion)stream.ReceiveNext();
            //Vector2 inputValue = (Vector2)stream.ReceiveNext();
            //h = inputValue.x;
            //v = inputValue.y;
            isTalking = (bool)stream.ReceiveNext();
            hpSync = (float)stream.ReceiveNext();   
        }
    }

    public void RPC_TakeDamege(float dmg, int viewID)
    {
        pv.RPC("TakeDamge", RpcTarget.AllBuffered, dmg, viewID);
    }

    [PunRPC]
    public void TakeDamage(float dmg, int veiwID)
    {
 
        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        healthUI.SetHpValue(currentHealth, maxHealth);

        if(currentHealth > 0)
        {
            // �ǰ� ȿ�� ó��
            // ī�޶� ���� ȿ���� �ش�.
            if (!isShaking && pv.IsMine)
            {
                StartCoroutine(ShakeCamera(5, 20, 0.3f));
            }

        }
        else
        {
            // ���� ó��
            DieProcess();
        }
    }

    IEnumerator ShakeCamera(float amplitude, float frequency, float duration)
    {
        isShaking = true;
        // duration��ŭ Perlin Noise�� ���� �����ͼ� �� ��ŭ x��� y���� ȸ����Ų��.
        float currentTime = 0;
        float delayTime = 1.0f / frequency;
        Quaternion originRot = Camera.main.transform.localRotation;

        while(currentTime < duration)
        {
            float range1 = Mathf.PerlinNoise1D(currentTime) - 0.5f;
            float range2 = Mathf.PerlinNoise1D(duration -currentTime) - 0.5f;
            float xRot = range1 * shakePower.x * amplitude;
            float yRot = range2 * shakePower.y * amplitude;

            Camera.main.transform.Rotate(xRot, yRot, 0);

            yield return new WaitForSeconds(delayTime);
            currentTime += delayTime;
        }

        Camera.main.transform.localRotation = originRot;
        isShaking = false;
    }

    void DieProcess()
    {
        if (pv.IsMine)
        {
            // ȭ���� ������� ó���Ѵ�.
            Volume currentVolume = FindAnyObjectByType<Volume>();
            ColorAdjustments postColor;
            currentVolume.profile.TryGet<ColorAdjustments>(out postColor);
            postColor.saturation.value = -100000;

            if (pv.IsMine)
            {
                UIManager.main_ui.btn_leave.gameObject.SetActive(true);
            }
        }

        // ���� �ִϸ��̼��� �����Ѵ�.
        myAnim.SetTrigger("DIE");
        // �ݶ��̴��� ��Ȱ��ȭ �Ѵ�.
        GetComponent<CapsuleCollider>().enabled = false;
        // �������� ���� ���·� ��ȯ�Ѵ�.
        playerState = PlayerState_.DIE;
        // �ִϸ��̼��� ������ �÷��̾ �����Ѵ�.
    }

    IEnumerator GoMainScene()
    {
        int currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.IsMasterClient && pv.IsMine && currentPlayers == maxPlayers)
        {
            requestLoadLevel = true; // �� ���� ��û�ϵ���
            yield return new WaitForSeconds(2.0f);

            // �濡 ������ �� ��ȣ�� �´� ������ �̵� �ϱ�
            PhotonNetwork.LoadLevel(2); // masterClient�� loadLevel�� �ص� �ٸ� �����鵵 �� ���� ��������� 
        }
    }
}
