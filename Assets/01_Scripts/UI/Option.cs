using TMPro;
using UnityEngine;
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
            _optionText.text = _optionText.text.Replace(highlightText, $"<color=#{colorCode}>"+ highlightText + "</color>");
        }

        else
        {
            Debug.Log($"{highlightText} 문자열을 찾을 수 없음");
        }
    }
}