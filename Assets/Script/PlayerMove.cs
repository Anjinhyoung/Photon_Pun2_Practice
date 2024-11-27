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

public class PlayerMove : PlayerState, IPunObservable , IInteractionInterface// i는 인터페이스니까 여기서 구현하고 있는 함수를 구현해야  한다. 
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

        // 현재 체력을 초기화한다.
        currentHealth = maxHealth;

        // 레이어를 변경한다.
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
            // 현재 말을 하고 있다면 보이스 아이콘을 활성화한다.
            voiceIcon.transform.gameObject.SetActive(voiceView.IsRecording);
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);

            // 현재 체력을 동기한다.
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
            stream.SendNext(voiceView.IsRecording);
            stream.SendNext(currentHealth);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는  상태라면...
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext(); // myPos가 네트워크를 통해 받은 위치로 업데이트 됨
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
            // 피격 효과 처리
            // 카메라를 흔드는 효과를 준다.
            if (!isShaking && pv.IsMine)
            {
                StartCoroutine(ShakeCamera(5, 20, 0.3f));
            }

        }
        else
        {
            // 죽음 처리
            DieProcess();
        }
    }

    IEnumerator ShakeCamera(float amplitude, float frequency, float duration)
    {
        isShaking = true;
        // duration만큼 Perlin Noise의 값을 가져와서 그 만큼 x축과 y축을 회전시킨다.
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
            // 화면을 흑백으로 처리한다.
            Volume currentVolume = FindAnyObjectByType<Volume>();
            ColorAdjustments postColor;
            currentVolume.profile.TryGet<ColorAdjustments>(out postColor);
            postColor.saturation.value = -100000;

            if (pv.IsMine)
            {
                UIManager.main_ui.btn_leave.gameObject.SetActive(true);
            }
        }

        // 죽은 애니메이션을 실행한다.
        myAnim.SetTrigger("DIE");
        // 콜라이더를 비활성화 한다.
        GetComponent<CapsuleCollider>().enabled = false;
        // 움직임을 죽음 상태로 전환한다.
        playerState = PlayerState_.DIE;
        // 애니메이션이 끝나면 플레이어를 제거한다.
    }

    IEnumerator GoMainScene()
    {
        int currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.IsMasterClient && pv.IsMine && currentPlayers == maxPlayers)
        {
            requestLoadLevel = true; // 한 번만 요청하도록
            yield return new WaitForSeconds(2.0f);

            // 방에 설정된 맵 번호에 맞는 씬으로 이동 하기
            PhotonNetwork.LoadLevel(2); // masterClient만 loadLevel을 해도 다른 유저들도 다 같이 따라와진다 
        }
    }
}
