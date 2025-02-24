using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = UnityEngine.ColorUtility;

public class Option : OutlineHighlight
{
    public string highlightText; // 강조할 문자
    private TextMeshProUGUI _optionText; // 문자 TMP
    public Color highlightColor; // 강조할 색상

    protected override void Awake()
    {
        base.Awake();
        _optionText = GetComponentInChildren<TextMeshProUGUI>();
        HighlightSetting();
    }
    
    private void HighlightSetting()
    {
        if (_optionText.text.Contains(highlightText))
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            _optionText.text = _optionText.text.Replace(highlightText, $"<b><color=#{colorCode}><size=\"40\">"+ highlightText + "</size></color></b>");
            // 색상 변경 <color=#컬러코드></color> / 폰트 굵기 <b></b> / 폰트 변경(Legacy만 가능) <font=\"폰트명\"><font>
            // 폰트 사이즈 변경 <size=\"변경할 크기\"></size>
        }

        else
        {
            Debug.Log($"{highlightText} 문자열을 찾을 수 없음");
        }
    }
}