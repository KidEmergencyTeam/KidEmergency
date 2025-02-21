using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject explanationPanel;       // 패널 (비활성화되어 있음)
    public TextMeshProUGUI explanationText;     // 패널 안에 있는 텍스트

    void Awake()
    {
        instance = this;
    }

    // 메시지에 따라 텍스트를 업데이트하고 패널을 활성화합니다.
    public void ShowExplanation(string message)
    {
        explanationText.text = message;       // 텍스트 내용 변경
        explanationPanel.SetActive(true);     // 패널 활성화
        Invoke("HideExplanation", 3f);        // 3초 후 패널 비활성화
    }

    void HideExplanation()
    {
        explanationPanel.SetActive(false);    // 패널 비활성화
    }
}
