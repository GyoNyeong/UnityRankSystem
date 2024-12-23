using UnityEngine;
using System.Collections.Generic;
public class RankDataQue
{
    // 외부 접근 제어. 접근 할 수있는 게더 추가. 싱글톤패턴으로 인스턴스 유지.
    private static RankDataQue instance;
    public static RankDataQue GetInstance
    {
        get 
        {
            if (instance == null)
            {
                instance = new RankDataQue();
            }
            return instance;
        }
    }

    private Queue<string> rankData;

    //Initialize
    private RankDataQue()
    {
        rankData = new Queue<string>();
    }

    //Add Data
    public void PushData(string data)
    {
        rankData.Enqueue(data);
        Debug.Log(data);
    }

    //Use Data
    public string GetData()
    {
        if(rankData.Count> 0)
        {
            return rankData.Dequeue();
        }
        else
        {
            return string.Empty;
        }
        
    }
}
