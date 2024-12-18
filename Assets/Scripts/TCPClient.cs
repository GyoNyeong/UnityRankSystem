using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading.Tasks;

public class TCPClient : MonoBehaviour
{
    public static TCPClient instance;

    private TcpClient tcpClient;
    private StreamReader reader;
    private StreamWriter writer;

    private bool isReceivingData = false; // 데이터를 한 번에 하나씩만 받도록 플래그 추가

    public string serverAddress = "127.0.0.1";
    public int port = 3000;


    public void ConnectToServer()
    {
        try
        {
            tcpClient = new TcpClient(serverAddress, port);
            writer = new StreamWriter(tcpClient.GetStream());
            reader = new StreamReader(tcpClient.GetStream());
            Debug.Log("Successfully connected to the server");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to connect to the server: " + e.Message);
        }
    }

    public void SendPlayerData(string jsonMessage)
    {
        if (tcpClient == null || !tcpClient.Connected)
        {
            Debug.Log("Not connected to the server. Trying to reconnect...");
            ConnectToServer();
        }

        if (tcpClient.Connected)
        {
            writer.WriteLine(jsonMessage);
            writer.Flush();
            Debug.Log("Transfer jsonMassage: " + jsonMessage);
        }
        else
        {
            Debug.LogError("Failed to send data: Not connected to the server.");
        }
    }


    // 서버에서 받은 데이터를 처리
    public async void ReceiveData()
    {
        if (isReceivingData) return; // 이미 데이터를 받고 있다면 종료
        isReceivingData = true; // 데이터 받기 시작

        try
        {
            Debug.Log("데이터 전송 대기중");
            // 비동기적으로 데이터가 준비될 때까지 기다림
            string response = await reader.ReadLineAsync();
            Debug.Log("데이터 생성 확인");
            if (response != null)
            {
                Debug.Log("Server response: " + response);

                // 서버에서 받은 데이터를 처리
                if (!string.IsNullOrEmpty(response))
                {
                    GameInstance.instance.SaveRankingData(response); // 서버에서 받은 JSON 데이터를 GameInstance에 전달
                }
            }
            else
            {
                // 서버에서 연결을 끊은 경우, 연결 종료 처리
                Debug.LogError("Connection lost. Server closed the connection.");
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Error while reading server response: " + ex.Message);
        }
        finally
        {
            isReceivingData = false; // 데이터 받기가 끝났으면 플래그 해제
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!tcpClient.Connected)
        {
            Debug.Log("Disconnected from server");
        }
    }
    private void OnApplicationQuit()
    {
        tcpClient.Close();
    }
}
