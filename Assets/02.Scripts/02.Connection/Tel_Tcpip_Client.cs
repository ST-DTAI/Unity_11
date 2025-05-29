
using System;
using System.Net.Sockets;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using System.Threading;



public class Tel_Tcpip_Client : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;
    private bool isReconThread = false;
    private Thread thread_Client;

    public string IP;
    public int Port;

    async void Start()
    {
        // await ConnectToServer();
        ThreadStart();
    }

    private void ThreadStart()
    {
        if (thread_Client != null)
        {
            //���� ���� �� return;
            isConnected = false;
            thread_Client.Join();
            thread_Client = null;
            return;
        }
        thread_Client = new Thread(ConnectToServer_Thread); // Thread ��ä ����, Form���� ���� �����忡�� connect �Լ��� �����.
        thread_Client.IsBackground = true; // Form�� ����Ǹ� thread1�� ����.
        thread_Client.Start(); // thread1 ����.
    }

    private async void ConnectToServer_Thread()  // thread1�� ����� �Լ�. ���������� ������ �����Ѵ�.
    {
        try
        {
            client = new TcpClient(IP, Port);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("������ ����Ǿ����ϴ�.");

            // ������ �����ϸ� �޽����� �����ϱ� �����մϴ�.
            ReceiveMSG();
        }
        catch (Exception e)
        {
            Debug.LogError("������ ���� ����: " + e.Message);
            await AttemptReconnect(); // �翬�� �õ�
        }
    }

    private async System.Threading.Tasks.Task ConnectToServer()
    {
        try
        {
            client = new TcpClient(IP, Port);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("������ ����Ǿ����ϴ�.");

            // ������ �����ϸ� �޽����� �����ϱ� �����մϴ�.
            ReceiveMSG();
        }
        catch (Exception e)
        {
            Debug.LogError("������ ���� ����: " + e.Message);
            await AttemptReconnect(); // �翬�� �õ�
        }
    }

    private async System.Threading.Tasks.Task AttemptReconnect()
    {
        if (isReconThread) { return; }
        isReconThread = true;
        while (!isConnected)
        {
            try
            {
                Debug.Log("�翬�� �õ� ��...");
                await ConnectToServer(); // �翬�� �õ�
                //ThreadStart();
                await System.Threading.Tasks.Task.Delay(2000); // 2�� �� ��õ�

            }
            catch (Exception ex)
            {
                Debug.LogError("�翬�� ����: " + ex.Message);
            }
        }
        isReconThread = false;
    }

    private async void ReceiveMSG()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogWarning("Ŭ���̾�Ʈ�� ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        try
        {
            byte[] buffer = new byte[256];
            while (isConnected) // ���� ���°� true�� ���� ��� ����
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("�����κ��� ���� ����: " + response);
                    }
                    else
                    {
                        Debug.LogWarning("������ ������ �����߽��ϴ�.");
                        isConnected = false; // ���� ���� ������Ʈ
                        await AttemptReconnect(); // ���� �������� �� �翬�� �õ�
                        break; // ������ ������ �������� ��� ������ Ż��
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("������ ���� �� ���� �߻�: " + ex.Message);
                    isConnected = false; // ���� ���� ������Ʈ
                    await AttemptReconnect(); // ���� �������� �� �翬�� �õ�
                    break; // ���� �߻� �� ���� Ż��
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("���� ���� �� ���� �߻�: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
            Debug.Log("Ŭ���̾�Ʈ ������ ����Ǿ����ϴ�.");
        }
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }
}
