using UnityEngine;
using TMPro;
using UnityEngine.Networking;
public class RankManager : MonoBehaviour
{
    [SerializeField] private GameObject rankTextPrefab;  // 랭킹을 표시할 텍스트 프리팹
    [SerializeField] private Transform contentTransform;  // ScrollView의 Content 객체

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateRankingUI();
  
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 랭킹 UI를 업데이트하는 메서드
    void UpdateRankingUI()
    {
        // 기존의 랭킹 항목들을 지웁니다. (스크롤뷰를 초기화)
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // GameInstance의 playerRanking을 사용하여 랭킹을 가져옴
        var playerRanking = GameInstance.instance.playerRanking;

        // rankTextPrefab을 기반으로 랭킹 항목을 동적으로 생성하여 ScrollView에 추가
        for (int i = 0; i < playerRanking.Count; i++)
        {
            var rank = playerRanking[i];

            // 텍스트 프리팹을 인스턴스화하여 contentTransform에 추가
            var rankText = Instantiate(rankTextPrefab, contentTransform);
            var textComponent = rankText.GetComponent<TextMeshProUGUI>();

            // 텍스트 컴포넌트의 텍스트를 설정
            textComponent.text = $"{i + 1}. {rank.playerName} - {rank.score}";
        }
    }
}

