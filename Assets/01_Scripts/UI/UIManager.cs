using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonManager<UIManager>
{
    public GameObject optionPanel;
    public OptionUI[] optionUI;
    public DialogUI dialogUI;
    public WarningUI warningUI;

    public Transform[] dialogPos;
    public Transform[] optionPos;
    public Transform[] warningPos;
    protected override void Awake()
    {
        base.Awake();
    }
    
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
        if (warningUI == null)
        {
            warningUI = FindAnyObjectByType<WarningUI>(FindObjectsInactive.Include);
        }
        warningUI.gameObject.SetActive(true);
    }

    public void SetWarningUI(Sprite image, string text)
    {
        if (warningUI == null)
        {
            warningUI = FindAnyObjectByType<WarningUI>(FindObjectsInactive.Include);
        }
        warningUI.warningImage.sprite = image;
        warningUI.warningText.text = text;
    }

    public void CloseWarningUI()
    {
        if (warningUI == null)
        {
            warningUI = FindAnyObjectByType<WarningUI>(FindObjectsInactive.Include);
        }
        warningUI.gameObject.SetActive(false);
    }

    #endregion

    #region UI Position Reset
    
    public void DialogPosReset(int index)
    {
        dialogUI.transform.SetParent(dialogPos[index]);
        dialogUI.transform.localPosition = Vector3.zero;
        dialogUI.transform.localEulerAngles = Vector3.zero;
        dialogUI.transform.localScale = Vector3.one;
    }

    public void OptionPosReset(int index)
    {
        optionPanel.transform.SetParent(optionPos[index]); 
        optionPanel.transform.localPosition = Vector3.zero;
        optionPanel.transform.localEulerAngles = Vector3.zero;
    }

    // public void WarningPosReset(int index)
    // {
    //     warningUI.transform.SetParent(warningPos[index]);
    //     warningUI.transform.localPosition = Vector3.zero;
    //     warningUI.transform.localEulerAngles = Vector3.zero;
    // }
    
    #endregion
}