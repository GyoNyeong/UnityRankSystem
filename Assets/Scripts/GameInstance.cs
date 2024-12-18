using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;

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
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        TCPClient.instance.ReceiveData();
    }

    [System.Serializable]
    public class Data
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
        public string status; // 서버의 응답 상태
        public RankData[] ranking; // 랭킹 데이터
    }

    public void SaveData(string playerNameData, int score)
    {
        Data data = new Data();
        data.playerName = playerNameData;
        data.point = score;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("SavedPoint : " + playerPoint);
        TCPClient.instance.SendPlayerData(json);

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
        TCPClient.instance.SendPlayerData(requestJson);   
    }

    public void SaveRankingData(string jsonResponse)
    {
        Debug.Log("랭킹데이터세이브진입");
        RankingResponse response = JsonUtility.FromJson<RankingResponse>(jsonResponse);
        playerRanking.Clear();  // 기존 랭킹 데이터를 지운 후

        if (response != null && response.ranking != null)
        {
            foreach (var rank in response.ranking)
            {
                playerRanking.Add(rank); // 랭킹 데이터를 playerRanking 리스트에 추가
            }
        }

        // 랭킹 데이터를 확인하기 위한 디버그 로그
        foreach (var rank in playerRanking)
        {
            Debug.Log("Player: " + rank.playerName + ", Score: " + rank.score);
        }
    }
}