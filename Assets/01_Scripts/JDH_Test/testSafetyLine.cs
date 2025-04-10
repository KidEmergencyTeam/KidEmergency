using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class testSafetyLine : MonoBehaviour
{
    [Header("�ڵ� �Ҵ�� Highlighter")]
    private Highlighter highlighter;

    [Header("XR ��Ʈ�ѷ� �� ���� �ڵ� Ž��")]
    private XRRayInteractor rightRayInteractor;
    private ActionBasedController rightController;

    private bool hasSelected = false;

    void Awake()
    {
        // �ڱ� �ڽſ��� Highlighter ������Ʈ �ڵ� �Ҵ�
        highlighter = GetComponent<Highlighter>();

        // ������ ù ��° XRRayInteractor�� ActionBasedController�� �ڵ� �Ҵ�
        rightRayInteractor = FindObjectOfType<XRRayInteractor>();
        rightController = FindObjectOfType<ActionBasedController>();

        if (highlighter == null)
            Debug.LogWarning("[testSafetyLine] Highlighter ������Ʈ�� ã�� �� �����ϴ�.");

        if (rightRayInteractor == null)
            Debug.LogWarning("[testSafetyLine] XRRayInteractor�� ã�� �� �����ϴ�.");

        if (rightController == null)
            Debug.LogWarning("[testSafetyLine] ActionBasedController�� ã�� �� �����ϴ�.");
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
        Debug.Log($"[testSafetyLine] '{gameObject.name}' ���̶����� Ȱ��ȭ��! (Blinking ON)");
    }
}
