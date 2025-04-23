using TMPro;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public string[] highlightText;
    public Color highlightColor;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void TextHighlight()
    {
        string result = popupText.text;

        if (highlightText != null)
        {
            foreach (string word in highlightText)
            {
                string colorCode = ColorUtility.ToHtmlStringRGB(highlightColor);
                result = result.Replace(word, $"<color=#{colorCode}>{word}</color>");
            }
        }
        
        popupText.text = result;
    }
}
