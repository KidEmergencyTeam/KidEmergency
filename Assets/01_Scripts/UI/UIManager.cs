using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    public OptionUI[] optionUI;
    public DialogUI dialogUI;

    #region Option
    public void SetOptionUI()
    {
        for (int i = 0; i < optionUI.Length; i++)
        {
            if (i < ActionManager.Instance.currentDialog.choices.Length)
            {
                Dialog.DialogChoice choice = ActionManager.Instance.currentDialog.choices[i];
                optionUI[i].optionText.text = choice.optionText;
                optionUI[i].optionImage.sprite = choice.optionSprite;
                optionUI[i].SetChoice(choice);
                optionUI[i].gameObject.SetActive(true);   
                
                print($"옵션 UI {i}번 세팅 완료");
            }
        }
    }

    public void CloseAllOptions()
    {
        for (int i = 0; i < optionUI.Length; i++)
        {
            optionUI[i].gameObject.SetActive(false);
        }
    }
    

    #endregion
}