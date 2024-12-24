using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class TCPClient
{
    public static TCPClient instance;
    //
    private Thread receiveThread;
    private TcpClient tcpClient;
    private StreamReader reader;
    private StreamWriter writer;
    private CancellationTokenSource cancellationTokenSource;

    public string serverAddress = "127.0.0.1";
    public int port = 3000;

    public TCPClient() 
    {
        receiveThread = new Thread(new ThreadStart(ReceiveDataThread));
    }

    public void StartThreads()
    {
        receiveThread.Start();
    }
    public void ConnectToServer()
    {
 
        try
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                tcpClient = new TcpClient(serverAddress, port);
                writer = new StreamWriter(tcpClient.GetStream());
                reader = new StreamReader(tcpClient.GetStream());
                Debug.Log("Successfully connected to the server");
            }
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
            Debug.Log("Data Send Failed : Not connected to the server.");
            ConnectToServer();
        }

        if (tcpClient.Connected)
        {
            writer.WriteLine(jsonMessage);
            writer.Flush();
            Debug.Log("Transfer jsonMassage: " + jsonMessage);
        }
    }

    public void ReceiveDataThread()
    {
        try
        {
            if (reader != null)
            {
                string jsonMassage = reader.ReadLine();
                Debug.Log("데이터 전송 확인");
                if (!string.IsNullOrEmpty(jsonMassage))
                {
                    RankDataQue.GetInstance.PushData(jsonMassage);
                }
            }
        }
        catch(Exception e)
        {

        }
    }

    public void Quit()
    {
        try
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                // StreamReader, StreamWriter와 TcpClient를 닫음
                reader.Close();
                writer.Close();
                tcpClient.Close();
                cancellationTokenSource.Cancel();
            } 
        }
        catch (Exception e)
        {

        }
    }
}
