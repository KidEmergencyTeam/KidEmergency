using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonManager<UIManager>
{
    public OptionUI[] optionUI;
    public DialogUI dialogUI;
    public WarningUI warningUI;
    public TitleUI titleUI;
    public PopupUI popupUI;

    #region Option
    public void SetOptionUI()
    {
        for (int i = 0; i < optionUI.Length; i++)
        {
            if (i < ActionManager.Instance.currentDialog.choices.Length)
            {
                DialogData.DialogChoice choice = ActionManager.Instance.currentDialog.choices[i];
                optionUI[i].optionText.text = choice.optionText;
                optionUI[i].optionImage.sprite = choice.optionSprite;
                optionUI[i].SetChoice(choice);
                optionUI[i].gameObject.SetActive(true);   
                
                print($"옵션 UI {i}번 세팅 완료");
            }
        }
    }

    public void CloseAllOptionUI()
    {
        for (int i = 0; i < optionUI.Length; i++)
        {
            optionUI[i].gameObject.SetActive(false);
        }
    }
    

    #endregion

    #region 경고 UI

    public void OpenWarningUI()
    {
        warningUI.gameObject.SetActive(true);
    }
    
    public void SetWarningUI(Sprite image, string text)
    {
        warningUI.warningImage.sprite = image;
        warningUI.warningText.text = text;
    }

    public void CloseWarningUI()
    {
        warningUI.gameObject.SetActive(false);
    }
    
    #endregion

    #region Title

    public IEnumerator ChangeScene() // 타이틀 씬에서 다른 게임 모드 씬으로 변경하는 코루틴
    {
        FadeInOut fade = FindObjectOfType<FadeInOut>();
        
        titleUI.gameObject.SetActive(false);
        
        yield return StartCoroutine(fade.FadeOut());
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(UIManager.Instance.titleUI.nextScene);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        StartCoroutine(fade.FadeIn());
        popupUI.gameObject.SetActive(false);
    }
    
    public void SetPopup(string text, string levelText, string modeText)
    {
        popupUI.popupText.text = text;
        popupUI.highlightText[0] = levelText;
        popupUI.highlightText[1] = modeText;
    }

    #endregion
}