using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image[] outline; // 버튼 충돌시 변경될 아웃라인
    private Color _originalOutlineColor; // 기본 아웃라인 색상

    protected virtual void Awake()
    {
        for (int i = 0; i < outline.Length; i++)
        {
            _originalOutlineColor = outline[i].color;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < outline.Length; i++)
        {
            outline[i].color = Color.green;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < outline.Length; i++)
        {
            outline[i].color = _originalOutlineColor;

        }
    }
}