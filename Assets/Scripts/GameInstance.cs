using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;
    private TCPClient clientSocket;
    private RankDataQue rankDataque;

    public string playerName;
    public int playerPoint;
    public List<RankData> playerRanking = new List<RankData>(); // 플레이어 랭킹 정보를 저장하는 리스트

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        clientSocket = TCPClient.instance ?? new TCPClient();
        rankDataque = RankDataQue.GetInstance;
        clientSocket.ConnectToServer();
        clientSocket.StartThreads();
        DontDestroyOnLoad(gameObject);

        RequestRanking();
    }

    // Update is called once per frame
    void Update()
    {
        string rankData = rankDataque.GetData();
        if (!string.IsNullOrEmpty(rankData))
        {
            SaveRankingData(rankData);
        }
    }

    [System.Serializable]
    private class Data
    {
        public string playerName;
        public int point;
    }

    [System.Serializable]
    public class RankData
    {
        public string playerName;
        public int score;
    }

    // 랭킹 데이터의 JSON 응답을 받을 때 사용하는 클래스
    [System.Serializable]
    public class RankingResponse
    {
        public RankData[] ranking;  // 여전히 배열로 사용합니다.
    }

    public void SaveData(string playerNameData, int score)
    {
        Data data = new Data();
        data.playerName = playerNameData;
        data.point = score;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("SavedPoint : " + playerPoint);
        clientSocket.SendPlayerData(json);

    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);

            playerName = data.playerName;
            playerPoint = data.point;
            Debug.Log("LoadPoint : " + playerPoint);
        }

    }

    public void RequestRanking()
    {
        // JSON 형식으로 요청 보내기
        string requestJson = "{\"action\": \"getRanking\"}";
        clientSocket.SendPlayerData(requestJson);
    }

    public void SaveRankingData(string jsonResponse)
    {
        try
        {
            // JSON 파싱
            RankingResponse response = JsonUtility.FromJson<RankingResponse>(jsonResponse);

            if (response != null && response.ranking != null)
            {
                playerRanking.Clear();  // 기존 랭킹 데이터 지우기
                foreach (var rank in response.ranking)
                {
                    playerRanking.Add(rank); // 랭킹 데이터를 playerRanking 리스트에 추가
                }
            }

            // 랭킹 데이터를 확인하기 위한 디버그 로그
            foreach (var rank in response.ranking)
            {
                Debug.Log("Player: " + rank.playerName + ", Score: " + rank.score);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while parsing ranking data: " + ex.Message);
        }
    }

    private void OnApplicationQuit()
    {
        clientSocket.Quit();
    }
}