using UnityEngine;

public class WarningUIController : MonoBehaviour
{
    [Header("경고 UI 참조")]
    public WarningUI warningUI;

    // 이미지 + 텍스트 변경
    public void SetWarning(Sprite image, string message)
    {
        if (warningUI == null) 
        {
            return;
        }

        // 이미지 변경
        if (warningUI.warningImage != null)
        {
            warningUI.warningImage.sprite = image;
        }

        // 텍스트 변경
        if (warningUI.warningText != null) 
        {
            warningUI.warningText.text = message;
        }
    }
}
