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
    public GameObject player;
    
    private Vector3 _originPos;

    protected override void Awake()
    {
        base.Awake();
        _originPos = player.transform.position;
    }

    private void Update()
    {
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

    public void WarningPosReset(int index)
    {
        warningUI.transform.SetParent(warningPos[index]);
        warningUI.transform.localPosition = Vector3.zero;
        warningUI.transform.localEulerAngles = Vector3.zero;
    }
    
    #endregion
}