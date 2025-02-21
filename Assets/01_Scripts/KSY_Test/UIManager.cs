using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI hintText;  // UI Text 오브젝트

    void Awake()
    {
        instance = this;
    }

    public void ShowHint(string message)
    {
        hintText.text = message;
        hintText.gameObject.SetActive(true);
        Invoke("HideHint", 3f);  // 3초 후 자동 사라짐
    }

    void HideHint()
    {
        hintText.gameObject.SetActive(false);
    }
}
