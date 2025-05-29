
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
            //연결 종료 후 return;
            isConnected = false;
            thread_Client.Join();
            thread_Client = null;
            return;
        }
        thread_Client = new Thread(ConnectToServer_Thread); // Thread 객채 생성, Form과는 별도 쓰레드에서 connect 함수가 실행됨.
        thread_Client.IsBackground = true; // Form이 종료되면 thread1도 종료.
        thread_Client.Start(); // thread1 시작.
    }

    private async void ConnectToServer_Thread()  // thread1에 연결된 함수. 메인폼과는 별도로 동작한다.
    {
        try
        {
            client = new TcpClient(IP, Port);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("서버에 연결되었습니다.");

            // 연결이 성공하면 메시지를 수신하기 시작합니다.
            ReceiveMSG();
        }
        catch (Exception e)
        {
            Debug.LogError("서버에 연결 실패: " + e.Message);
            await AttemptReconnect(); // 재연결 시도
        }
    }

    private async System.Threading.Tasks.Task ConnectToServer()
    {
        try
        {
            client = new TcpClient(IP, Port);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("서버에 연결되었습니다.");

            // 연결이 성공하면 메시지를 수신하기 시작합니다.
            ReceiveMSG();
        }
        catch (Exception e)
        {
            Debug.LogError("서버에 연결 실패: " + e.Message);
            await AttemptReconnect(); // 재연결 시도
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
                Debug.Log("재연결 시도 중...");
                await ConnectToServer(); // 재연결 시도
                //ThreadStart();
                await System.Threading.Tasks.Task.Delay(2000); // 2초 후 재시도

            }
            catch (Exception ex)
            {
                Debug.LogError("재연결 실패: " + ex.Message);
            }
        }
        isReconThread = false;
    }

    private async void ReceiveMSG()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogWarning("클라이언트가 연결되어 있지 않습니다.");
            return;
        }

        try
        {
            byte[] buffer = new byte[256];
            while (isConnected) // 연결 상태가 true인 동안 계속 수신
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("서버로부터 받은 응답: " + response);
                    }
                    else
                    {
                        Debug.LogWarning("서버가 연결을 종료했습니다.");
                        isConnected = false; // 연결 상태 업데이트
                        await AttemptReconnect(); // 연결 끊어졌을 때 재연결 시도
                        break; // 서버가 연결을 종료했을 경우 루프를 탈출
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("데이터 수신 중 오류 발생: " + ex.Message);
                    isConnected = false; // 연결 상태 업데이트
                    await AttemptReconnect(); // 연결 끊어졌을 때 재연결 시도
                    break; // 오류 발생 시 루프 탈출
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("응답 수신 중 예외 발생: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
            Debug.Log("클라이언트 연결이 종료되었습니다.");
        }
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }
}
