using UnityEngine;
using TMPro;
public class PlayerNameText : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerNameText.text = "Name";
        playerNameText.onEndEdit.AddListener(SetPlayerName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPlayerName(string playerName)
    {
        GameInstance.instance.playerName = playerName;

    }
}
