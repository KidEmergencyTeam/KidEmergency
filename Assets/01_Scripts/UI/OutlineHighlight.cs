using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image outline; // 버튼 충돌시 변경될 아웃라인
    public Color _originalOutlineColor; // 기본 아웃라인 색상

    protected virtual void Awake()
    {
        _originalOutlineColor = outline.color;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TitleUI.Instance != null)
        {
            if (!TitleUI.Instance.IsPopupOpen())
            {
                outline.color = Color.green;
            }

            if (TitleUI.Instance.IsPopupOpen() && CompareTag("PopupUI"))
            {
                outline.color = Color.green;
            }
        }
        else
        {
            outline.color = Color.green;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if (TitleUI.Instance != null)
        // {
        //     if (!TitleUI.Instance.IsPopupOpen())
        //     {
        //         outline.color = _originalOutlineColor;
        //     }
        // }
        // else
        // {
        //     outline.color = _originalOutlineColor;
        // }
        
        outline.color = _originalOutlineColor;
    }
}