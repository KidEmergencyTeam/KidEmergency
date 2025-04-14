using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class testSafetyLine : MonoBehaviour
{
    [Header("하이라이터 (필수)")]
    [SerializeField] private Highlighter highlighter;

    [Header("XR 컨트롤러와 레이 (필수)")]
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
            Debug.Log("[testSafetyLine] Select 이벤트 연결됨.");
        }
    }

    private void OnDisable()
    {
        if (rightController != null &&
            rightController.selectAction != null &&
            rightController.selectAction.action != null)
        {
            rightController.selectAction.action.performed -= OnSelectPerformed;
            Debug.Log("[testSafetyLine] Select 이벤트 해제됨.");
        }
    }

    private void Start()
    {
        if (highlighter == null || rightController == null || rightRayInteractor == null)
        {
            Debug.LogError("[testSafetyLine] 필수 컴포넌트가 누락되었습니다!");
            enabled = false;
            return;
        }

        Debug.Log("[testSafetyLine] 컴포넌트 초기화 완료. 이벤트 대기 중...");
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.DrawLine(rightRayInteractor.transform.position, hit.point, Color.green);

            if (hit.transform == transform)
            {
                Debug.Log("[testSafetyLine] Select 입력 발생 + Ray로 오브젝트 선택됨 → 하이라이터 비활성화");
                DisableHighlighter();
            }
            else
            {
                Debug.Log($"[testSafetyLine] Ray는 '{hit.transform.name}'을 가리키고 있음 (대상 아님)");
            }
        }
        else
        {
            Debug.Log("[testSafetyLine] Select 입력은 발생했지만, Ray가 아무 대상도 가리키지 않음.");
        }
    }

    private void DisableHighlighter()
    {
        highlighter.gameObject.SetActive(false);
        objSelected = true;
    }

}
