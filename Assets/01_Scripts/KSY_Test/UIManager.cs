using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI explanationText;

    void Awake()
    {
        instance = this;
    }

    public void ShowExplanation(string message)
    {
        explanationText.text = message;
        explanationText.gameObject.SetActive(true);

        // 메시지 출력후
        // 3초 후 비활성화
        Invoke("HideExplanation", 3f);  
    }

    void HideExplanation()
    {
        explanationText.gameObject.SetActive(false);
    }
}
