using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;

public class VoiceManager : MonoBehaviourPun
{
    Recorder recorder;

    void Start()
    {
        recorder = GetComponent<Recorder>();
    }

    void Update()
    {
        // ����, MŰ�� ������ ���Ұ��Ѵ�.
        if (Input.GetKeyDown(KeyCode.M))
        {

            recorder.TransmitEnabled = !recorder.TransmitEnabled;
        }



        if (Input.GetKeyDown(KeyCode.Alpha0))
        {

            recorder.TargetPlayers = new int[] { (int)KeyCode.Alpha0 - 48 };
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            recorder.TargetPlayers = new int[] { (int)KeyCode.Alpha1 - 48 };
        }
    }
}