using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class OptionUI : OutlineHighlight
{
    public TextMeshProUGUI optionText; // 문자 TMP
    public string highlightText; // 강조할 문자
    public Image optionImage; // 옵션 이미지
    public Color highlightColor; // 강조할 문자 색상

    private Animator _anim;
    private DialogData.DialogChoice _myChoice;
    private Button _button;
    
    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OptionClicked);
    }

    #region 장소영 옵션 선택

    public void SetChoice(DialogData.DialogChoice choice)
    {
        _myChoice = choice;
        highlightText = choice.highlightText;
        HighlightSetting();
    }
    
    private void OptionClicked()
    {
        RobotController robot = FindObjectOfType<RobotController>();
        if (_myChoice != null)
        {
            UIManager.Instance.CloseAllOptionUI();
            outline.color = _originalOutlineColor;

            ActionManager.Instance.currentDialog = _myChoice.nextOptionDialog;
            ActionManager.Instance.currentAction = _myChoice.choiceOptionNextActionType;
            
            ActionManager.Instance.showOptionAction.CompleteAction();
            if (_myChoice.isAnswer)
            {
                robot.SetHappy();
            }
            else
            {
                robot.SetAngry();
            }
        }
    }

    #endregion

    private void HighlightSetting()
    {
        if (optionText.text.Contains(highlightText))
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            optionText.text = optionText.text.Replace(highlightText,
                $"<b><color=#{colorCode}><size=\"78\">" + highlightText + "</size></color></b>");
            // 색상 변경 <color=#컬러코드></color> / 폰트 굵기 <b></b> / 폰트 변경(Legacy만 가능) <font=\"폰트명\"><font>
            // 폰트 사이즈 변경 <size=\"변경할 크기\"></size>
        }

        else
        {
            Debug.Log($"{highlightText} 문자열을 찾을 수 없음");
        }
    }

    private void StartAnimation() // OptionClicked 메서드에 넣을 예정
    {
        if (true) // 옵션 선택지에 따라
        {
            _anim.SetTrigger("Selected");
        }
        
        // else if (true)
        // {
        //     anim.SetTrigger("Unselected");
        // }
}

}