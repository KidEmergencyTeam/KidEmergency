using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public string[] highlightText;
    public Color highlightColor;

    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        yesButton.onClick.AddListener(YesButtonClicked);
        noButton.onClick.AddListener(NoButtonClicked);
    }

    private void Start()
    {
        TextHighlight();
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
            StartCoroutine(ChangeScene());
        }
    }
    
    private void NoButtonClicked()
    {
        this.gameObject.SetActive(false);
    }
    
    private IEnumerator ChangeScene()
    {
        UIManager.Instance.titleUI.gameObject.SetActive(false);
        
        StartCoroutine(FadeInOut.Instance.FadeOut());
        yield return new WaitForSeconds(1.5f);
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(UIManager.Instance.titleUI.nextScene);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        StartCoroutine(FadeInOut.Instance.FadeIn());
        yield return new WaitForSeconds(1.5f);
    }
    
    public void TextHighlight()
    {
        string result = popupText.text;

        foreach (string word in highlightText)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            result = result.Replace(word, $"<color=#{colorCode}>{word}</color>");
        }
        
        popupText.text = result;
    }
}
