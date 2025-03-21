using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRUIOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline outline;
    private Color outlineColor = Color.green;

    [Header("두께")]
    public Vector2 outlineDistance = new Vector2(5, 5);

    void Awake()
    {
        // 해당 오브젝트에 Outline 컴포넌트가 없다면 추가
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // 색상 적용 -> 초록색 고정
        outline.effectColor = outlineColor;

        // 두께 적용 -> 인스펙터에서 설정 가능
        outline.effectDistance = outlineDistance;

        // 투명도 반영
        outline.useGraphicAlpha = true;

        // 게임 시작 시 아웃라인 비활성화
        outline.enabled = false;
    }

    // 포인터가 UI 위에 올라왔을 때 아웃라인 활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    // 포인터가 UI에서 벗어났을 때 아웃라인 비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}
