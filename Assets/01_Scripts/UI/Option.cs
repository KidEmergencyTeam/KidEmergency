using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class Option : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image outline; // 버튼 충돌시 변경될 아웃라인
    private Color originalOutlineColor; // 기본 아웃라인 색상
    public string highlightText; // 강조할 문자
    public TMP_Text optionText; // 문자 TMP
    public Color highlightColor; // 강조할 색상
    
    private void Start()
    {
        originalOutlineColor = outline.color;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.color = originalOutlineColor;
    }
}