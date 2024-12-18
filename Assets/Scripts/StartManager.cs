using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class StartManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestPointText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bestPointText.text = "BestPoint";
        if(GameInstance.instance.playerPoint != 0)
        {
            bestPointText.text = "BestPoint : " + GameInstance.instance.playerPoint.ToString();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
   
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
