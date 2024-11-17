using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Net.Sockets; // ��� �� �� �ʿ���
using System.Text;  // ���ڵ��� �� �ʿ���
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

    // ��ſ� ������ �����
    void InitializeUDPThread()
    {
        // ��׶��忡�� �� Thread�� �����ϰ� �ʹ�. (��ſ� ������)
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true;
        udpThread.Start();
    }

    // �� �Լ��� ���� ���� (�޴� ����)
    void ReceiveData()
    {
        // ���� ���� �� ���� Ŭ���̾�Ʈ�� �����Ѵ�.
        receivePort = new UdpClient(portNumber); // ��Ʈ��ȣ���� UDP Ŭ���̾�Ʈ ������ �����Ͽ� �����͸� ���� �غ�
        IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, portNumber); // ������ ��ġ
        try
        {
            while (true)
            {
                // ��� ����� ���̳ʸ� �����͸� �޴´�.
                byte[] bins = receivePort.Receive(ref remoteClient);
                string binaryString = Encoding.UTF8.GetString(bins);
                print($"���� ������:{binaryString}");

            }
        }
        catch(SocketException message)
        {
            // ��� ���� �ڵ� �� ���� ������ ����Ѵ�.
            Debug.LogError($"Error Code: {message.ErrorCode} - {message}");
        }

        finally
        {
            receivePort.Close();
        }
    }

    private void OnDisable()
    {
        // UDP ��Ʈ���� �����Ѵ�.
        receivePort.Close();    
    }
    void Update()
    {
        
    }
}
