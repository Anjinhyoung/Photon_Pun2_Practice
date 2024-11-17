using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Net.Sockets; // 통신 할 때 필요함
using System.Text;  // 인코딩할 때 필요함
using System.Net;

public class UDPConnector : MonoBehaviour
{
    public int portNumber = 5000;
    Thread udpThread;
    UdpClient receivePort;

    void Start()
    {
        InitializeUDPThread();
    }

    // 통신용 스레드 만들기
    void InitializeUDPThread()
    {
        // 백그라운드에서 새 Thread를 실행하고 싶다. (통신용 스레드)
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true;
        udpThread.Start();
    }

    // 이 함수는 서버 역할 (받는 역할)
    void ReceiveData()
    {
        // 서버 오픈 및 원격 클라이언트를 설정한다.
        receivePort = new UdpClient(portNumber); // 포트번호에서 UDP 클라이언트 소켓을 생성하여 데이터를 수신 준비
        IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, portNumber); // 수신할 위치
        try
        {
            while (true)
            {
                // 통신 결과를 바이너리 데이터를 받는다.
                byte[] bins = receivePort.Receive(ref remoteClient);
                string binaryString = Encoding.UTF8.GetString(bins);
                print($"받은 데이터:{binaryString}");

            }
        }
        catch(SocketException message)
        {
            // 통신 에러 코드 및 에러 내용을 출력한다.
            Debug.LogError($"Error Code: {message.ErrorCode} - {message}");
        }

        finally
        {
            receivePort.Close();
        }
    }

    private void OnDisable()
    {
        // UDP 스트림을 종료한다.
        receivePort.Close();    
    }
    void Update()
    {
        
    }
}
