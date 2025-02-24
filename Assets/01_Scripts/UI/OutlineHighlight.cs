using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image outline; // 버튼 충돌시 변경될 아웃라인
    private Color originalOutlineColor; // 기본 아웃라인 색상
    private void Start()
    {
        originalOutlineColor = outline.color;
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