using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class testSafetyLine : MonoBehaviour
{
    [Header("자동 할당된 Highlighter")]
    private Highlighter highlighter;

    [Header("XR 컨트롤러 및 레이 자동 탐색")]
    private XRRayInteractor rightRayInteractor;
    private ActionBasedController rightController;

    private bool hasSelected = false;

    void Awake()
    {
        // 자기 자신에서 Highlighter 컴포넌트 자동 할당
        highlighter = GetComponent<Highlighter>();

        // 씬에서 첫 번째 XRRayInteractor와 ActionBasedController를 자동 할당
        rightRayInteractor = FindObjectOfType<XRRayInteractor>();
        rightController = FindObjectOfType<ActionBasedController>();

        if (highlighter == null)
            Debug.LogWarning("[testSafetyLine] Highlighter 컴포넌트를 찾을 수 없습니다.");

        if (rightRayInteractor == null)
            Debug.LogWarning("[testSafetyLine] XRRayInteractor를 찾을 수 없습니다.");

        if (rightController == null)
            Debug.LogWarning("[testSafetyLine] ActionBasedController를 찾을 수 없습니다.");
    }

    void Update()
    {
        if (hasSelected || highlighter == null || rightRayInteractor == null || rightController == null) return;

        bool isHovered = rightRayInteractor.hasHover;
        bool isSelectPressed = rightController.selectAction.action.ReadValue<float>() > 0.5f;

        if (isHovered && isSelectPressed)
        {
            ActivateHighlighter();
        }
    }

    private void ActivateHighlighter()
    {
        hasSelected = true;
        highlighter.isBlinking = true;
        Debug.Log($"[testSafetyLine] '{gameObject.name}' 하이라이터 활성화됨! (Blinking ON)");
    }
}
