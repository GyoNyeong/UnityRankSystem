using UnityEngine;

public class RankManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameInstance.instance.RequestRanking();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
