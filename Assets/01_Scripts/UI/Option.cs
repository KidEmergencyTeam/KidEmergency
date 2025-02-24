using TMPro;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

public class Option : OutlineHighlight
{
    public string highlightText; // 강조할 문자
    public TMP_Text optionText; // 문자 TMP
    public Color highlightColor; // 강조할 색상
    
    private void Start()
    {
        HighlightSetting();
    }
    
    private void HighlightSetting()
    {
        if (optionText.text.Contains(highlightText))
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
            optionText.text = optionText.text.Replace(highlightText, $"<color=#{colorCode}>"+ highlightText + "</color>");
        }

        else
        {
            Debug.Log($"{highlightText} 문자열을 찾을 수 없음");
        }
    }
}