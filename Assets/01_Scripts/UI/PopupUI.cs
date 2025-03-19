using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    [FormerlySerializedAs("text")] public TextMeshProUGUI popupText;
    public string[] highlightText;
    public Color highlightColor;

    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        yesButton.onClick.AddListener(YesButtonClicked);
        noButton.onClick.AddListener(NoButtonClicked);
    }

    private void YesButtonClicked()
    {
        this.gameObject.SetActive(false);
        if (UIManager.Instance.titleUI.currentMenu == "Exit")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        else
        {
            ChangeScene();
        }
    }
    
    private void NoButtonClicked()
    {
        this.gameObject.SetActive(false);
    }
    
    private IEnumerator ChangeScene()
    {
        StartCoroutine(FadeInOut.Instance.FadeOut());
        yield return new WaitForSeconds(1.5f);
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync("");
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        StartCoroutine(FadeInOut.Instance.FadeIn());
        yield return new WaitForSeconds(1.5f);
    }
    
    public string TextHighlight()
    {
        string result = popupText.text;

        foreach (string word in highlightText)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            result = result.Replace(word, $"<color=#{colorCode}>{word}</color>");
        }

        return result;
    }
}
