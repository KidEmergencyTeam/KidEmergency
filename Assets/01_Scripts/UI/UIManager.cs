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

    private GameObject camOffset;
    private Vector3 _originPos;

    protected override void Awake()
    {
        base.Awake();
        camOffset = GameObject.Find("Camera Offset");
        _originPos = camOffset.transform.position;
    }

    private void Update()
    {
        SetUIPosition();
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

    #region UI Position Reset

    public void SetUIPosition()
    {
        if (SceneManager.GetActiveScene().name == "JSY_School")
        {
            if (camOffset != null)
            {
                RobotController seti = FindObjectOfType<RobotController>();
                if (camOffset.transform.position == _originPos)
                {
                    DialogPosReset(0);
                    WarningPosReset(1);
                    seti.SetRobotPos(seti.setiPos[0]);
                }

                else
                {
                    DialogPosReset(1);
                    WarningPosReset(0);
                    seti.SetRobotPos(seti.setiPos[1]);
                }
            }
            
            else return;
        }

        if(SceneManager.GetActiveScene().name == "JSY_SchoolHall")
        {
            DialogPosReset(2);
            OptionPosReset(1);
            WarningPosReset(2);
        }
        
        else if (SceneManager.GetActiveScene().name == "JSY_StairEv")
        {
            DialogPosReset(3);
            OptionPosReset(2);
            WarningPosReset(3);
        }
        
        else if (SceneManager.GetActiveScene().name == "JSY_SchoolGround")
        {
            DialogPosReset(4);
        }
    }

    private void DialogPosReset(int index)
    {
        dialogUI.transform.SetParent(dialogPos[index]);
        dialogUI.transform.localPosition = Vector3.zero;
        dialogUI.transform.localEulerAngles = Vector3.zero;
    }

    private void OptionPosReset(int index)
    {
        optionPanel.transform.SetParent(optionPos[index]); 
        optionPanel.transform.localPosition = Vector3.zero;
        optionPanel.transform.localEulerAngles = Vector3.zero;
    }

    private void WarningPosReset(int index)
    {
        warningUI.transform.SetParent(warningPos[index]);
        warningUI.transform.localPosition = Vector3.zero;
        warningUI.transform.localEulerAngles = Vector3.zero;
    }
    
    #endregion
}