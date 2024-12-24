using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class StartManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestPointText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var best = GameInstance.instance.playerRanking[0];
        bestPointText.text = $"BestPoint \n {best.playerName} : {best.score} ";
    }

    public void GameStart()
    {
        SceneManager.LoadScene("main");
    }

    public void MoveRankScene()
    {
        SceneManager.LoadScene("Rank");
    }


    public void QuitGame()
    {
        //에디터 또는 퍼빌리싱된 실행파일 종료
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
