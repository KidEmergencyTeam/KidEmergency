using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class OptionUI : OutlineHighlight
{
    public TextMeshProUGUI optionText; // 문자 TMP
    public string highlightText; // 강조할 문자
    public Image optionImage; // 옵션 이미지
    public Color highlightColor; // 강조할 색상

    private Animator _anim;
    private Dialog.DialogChoice _myChoice;
    private Button _button;
    
    protected override void Awake()
    {
        base.Awake();
        _anim = GetComponent<Animator>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OptionClicked);
        gameObject.SetActive(false);
    }
    
    public void SetChoice(Dialog.DialogChoice choice)
    {
        _myChoice = choice;
        highlightText = choice.highlightText;
        HighlightSetting();
    }
    
    private void OptionClicked()
    {
    if (_myChoice != null)
    {
        ActionManager.Instance.currentDialog = _myChoice.nextOptionDialog;
        ActionManager.Instance.currentAction = _myChoice.choiceOptionNextActionType;
        
        UIManager.Instance.CloseAllOptions();
        
        ActionManager.Instance.UpdateAction();
    }
    }

    private void HighlightSetting()
    {
        if (optionText.text.Contains(highlightText))
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            optionText.text = optionText.text.Replace(highlightText,
                $"<b><color=#{colorCode}><size=\"40\">" + highlightText + "</size></color></b>");
            // 색상 변경 <color=#컬러코드></color> / 폰트 굵기 <b></b> / 폰트 변경(Legacy만 가능) <font=\"폰트명\"><font>
            // 폰트 사이즈 변경 <size=\"변경할 크기\"></size>
        }

        else
        {
            Debug.Log($"{highlightText} 문자열을 찾을 수 없음");
        }
    }

    private void StartAnimation()
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