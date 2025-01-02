using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using UnityEngine.UIElements;

public class TCPClient
{
    public static TCPClient instance;
    //
    private Thread receiveThread;
    private TcpClient tcpClient;
    private StreamReader reader;
    private StreamWriter writer;
    private CancellationTokenSource cancellationTokenSource; // 취소 요청을 실행하는 객체

    public string serverAddress = "127.0.0.1"; //에디터에서 조정가능하도록 public으로 설정
    public int port = 2000;

    public TCPClient()
    {
        //생성자에서 쓰레드 생성. 소켓 객체 생성시 쓰레드도 생성됨.
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
            //writer.WriteLine(jsonMessage);
            //writer.Flush();
            //Debug.Log("Transfer jsonMassage: " + jsonMessage);

            ushort length = (ushort)jsonMessage.Length;
            byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)length));
            tcpClient.GetStream().Write(lengthBytes, 0, lengthBytes.Length);

            // 그 뒤에 JSON 데이터를 보냄
            byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
            tcpClient.GetStream().Write(jsonData, 0, jsonData.Length);
            Debug.Log("SendMessage : " + jsonMessage);
            Debug.Log("SendData : " + BitConverter.ToString(jsonData));
        }
    }
    //public void SendPlayerData(string jsonMessage)
    //{
    //    // 연결이 끊어졌거나, writer/reader가 null이라면 재연결 시도
    //    if (tcpClient == null || !tcpClient.Connected || writer == null || reader == null)
    //    {
    //        Debug.Log("Data Send Failed: Not connected to the server.");
    //        ConnectToServer();
    //    }

    //    // 재연결 후에도 연결이 되지 않았다면 에러 처리
    //    if (tcpClient.Connected)
    //    {
    //        try
    //        {
    //            // 보내는 데이터의 길이를 먼저 보냄
    //            ushort length = (ushort)jsonMessage.Length;
    //            byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)length));
    //            tcpClient.GetStream().Write(lengthBytes, 0, lengthBytes.Length);

    //            // JSON 데이터를 보냄
    //            byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
    //            tcpClient.GetStream().Write(jsonData, 0, jsonData.Length);
    //            Debug.Log("SendMessage : " + jsonMessage);
    //            Debug.Log("SendData : " + BitConverter.ToString(jsonData));
    //        }
    //        catch (IOException e)
    //        {
    //            Debug.LogError("IOException: Unable to write data to the transport connection. " + e.Message);
    //            // 연결이 끊어졌을 경우 재연결 시도
    //            ConnectToServer();
    //        }
    //        catch (SocketException e)
    //        {
    //            Debug.LogError("SocketException: " + e.Message);
    //            // 연결이 끊어졌을 경우 재연결 시도
    //            ConnectToServer();
    //        }
    //    }
    //}
    public void ReceiveDataThread()
    {
        //try
        //{
        //    if (reader != null)
        //    {
        //        string jsonMassage = reader.ReadLine();
        //        Debug.Log("데이터 전송 확인");
        //        if (!string.IsNullOrEmpty(jsonMassage))
        //        {
        //            RankDataQue.GetInstance.PushData(jsonMassage);
        //        }
        //    }
        //}
        //catch
        //{

        //}
 
        // Length 수신 (2바이트)
        byte[] lengthBytes = new byte[2]; // 길이 정보 수신을 위한 배열
        int recvByte = tcpClient.GetStream().Read(lengthBytes, 0, lengthBytes.Length);

        // 네트워크 바이트 순서에서 호스트 바이트 순서로 변환
        ushort length = BitConverter.ToUInt16(lengthBytes, 0);
        length = (ushort)IPAddress.NetworkToHostOrder((short)length);

        // 실제 데이터 수신
        byte[] buffer = new byte[length];
        int totalBytes = 0;

        while (totalBytes < length)
        {
            int bytesToRead = length - totalBytes;
            recvByte = tcpClient.GetStream().Read(buffer, totalBytes, bytesToRead);

            if (recvByte == 0)
            {
                throw new Exception("No data received");
            }

            totalBytes += recvByte;
        }

        // UTF-8로 데이터를 읽어서 문자열로 변환
        string receivedData = System.Text.Encoding.UTF8.GetString(buffer);
        Debug.Log("Received data: " + receivedData);

        RankDataQue.GetInstance.PushData(receivedData);
    }

    public void Quit()
    {
        try
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                // StreamReader, StreamWriter와 TcpClient를 닫음.쓰레드 삭제.
                reader.Close();
                writer.Close();
                tcpClient.Close();
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }
        catch
        {

        }
    }
}
