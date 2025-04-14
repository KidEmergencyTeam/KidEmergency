using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class testSafetyLine : MonoBehaviour
{
    [Header("���̶����� (�ʼ�)")]
    [SerializeField] private Highlighter highlighter;

    [Header("XR ��Ʈ�ѷ��� ���� (�ʼ�)")]
    [SerializeField] private ActionBasedController rightController;
    [SerializeField] private XRRayInteractor rightRayInteractor;

    public bool objSelected = false;

    private void OnEnable()
    {
        if (rightController != null &&
            rightController.selectAction != null &&
            rightController.selectAction.action != null)
        {
            rightController.selectAction.action.performed += OnSelectPerformed;
            Debug.Log("[testSafetyLine] Select �̺�Ʈ �����.");
        }
    }

    private void OnDisable()
    {
        if (rightController != null &&
            rightController.selectAction != null &&
            rightController.selectAction.action != null)
        {
            rightController.selectAction.action.performed -= OnSelectPerformed;
            Debug.Log("[testSafetyLine] Select �̺�Ʈ ������.");
        }
    }

    private void Start()
    {
        if (highlighter == null || rightController == null || rightRayInteractor == null)
        {
            Debug.LogError("[testSafetyLine] �ʼ� ������Ʈ�� �����Ǿ����ϴ�!");
            enabled = false;
            return;
        }

        Debug.Log("[testSafetyLine] ������Ʈ �ʱ�ȭ �Ϸ�. �̺�Ʈ ��� ��...");
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.DrawLine(rightRayInteractor.transform.position, hit.point, Color.green);

            if (hit.transform == transform)
            {
                Debug.Log("[testSafetyLine] Select �Է� �߻� + Ray�� ������Ʈ ���õ� �� ���̶����� ��Ȱ��ȭ");
                DisableHighlighter();
            }
            else
            {
                Debug.Log($"[testSafetyLine] Ray�� '{hit.transform.name}'�� ����Ű�� ���� (��� �ƴ�)");
            }
        }
        else
        {
            Debug.Log("[testSafetyLine] Select �Է��� �߻�������, Ray�� �ƹ� ��� ����Ű�� ����.");
        }
    }

    private void DisableHighlighter()
    {
        highlighter.gameObject.SetActive(false);
        objSelected = true;
    }

}
