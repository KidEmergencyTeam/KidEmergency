using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RayController2 : MonoBehaviour
{
    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // 좌측, 우측 컨트롤러의 Select 액션 -> 그립 버튼
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    [Header("XR Ray Interactor")]
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;

    [Header("XR Interactor Line Visual")]
    [SerializeField] private XRInteractorLineVisual _leftLine;
    [SerializeField] private XRInteractorLineVisual _rightLine;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    // 현재 활성화된 레이가 오른손인지 여부 (true: 오른손, false: 왼손)
    private bool isRightActive = true;

    private void Start()
    {
        // 초기 상태: 오른손 관련 컴포넌트만 활성화, 좌측은 비활성화
        isRightActive = true;

        _rightRay.enabled = true;
        _rightLine.enabled = true;
        _rightLineRenderer.enabled = true;

        _leftRay.enabled = false;
        _leftLine.enabled = false;
        _leftLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 이름으로 좌측 그립 액션 찾기 
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);

            // 액션이 존재하면 이벤트 등록
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += OnSelectActionPerformed;
                leftSelectAction.Enable();
                Debug.Log("[RayController2] 좌측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("[RayController2] 좌측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }

            // 이름으로 우측 그립 액션 찾기 
            rightSelectAction = inputActionAsset.FindAction("XRI RightHand Interaction/Select", true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectActionPerformed;
                rightSelectAction.Enable();
                Debug.Log("[RayController2] 우측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("[RayController2] 우측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[RayController2] InputActionAsset이 할당되지 않았습니다.");
        }
    }

    private void OnDisable()
    {
        if (leftSelectAction != null)
        {
            leftSelectAction.performed -= OnSelectActionPerformed;
            leftSelectAction.Disable();
        }
        if (rightSelectAction != null)
        {
            rightSelectAction.performed -= OnSelectActionPerformed;
            rightSelectAction.Disable();
        }
    }

    // 현재 비활성화된 컨트롤러의 그립 입력이 들어오면 레이를 전환
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        // 좌측 그립 입력 시
        if (context.action == leftSelectAction)
        {
            // 우측 레이 활성화 상태라면
            if (isRightActive)
            {
                // 좌측 레이 활성화
                SwitchLeftRay();
            }
        }
        // 우측 그립 입력 시
        else if (context.action == rightSelectAction)
        {
            // 좌측 레이 활성화 상태라면
            if (!isRightActive)
            {
                // 우측 레이 활성화
                SwitchRightRay();
            }
        }
    }

    // 좌측 레이 활성화, 우측 레이 비활성화
    private void SwitchLeftRay()
    {
        isRightActive = false;

        _rightRay.enabled = false;
        _rightLine.enabled = false;
        _rightLineRenderer.enabled = false;

        _leftRay.enabled = true;
        _leftLine.enabled = true;
        _leftLineRenderer.enabled = true;
    }

    // 우측 레이 활성화, 좌측 레이 비활성화
    private void SwitchRightRay()
    {
        isRightActive = true;

        _leftRay.enabled = false;
        _leftLine.enabled = false;
        _leftLineRenderer.enabled = false;

        _rightRay.enabled = true;
        _rightLine.enabled = true;
        _rightLineRenderer.enabled = true;
    }

    // 선택지 처리 후 InputAction을 재활성화
    public void ReactivateInputActions()
    {
        if (leftSelectAction != null && !leftSelectAction.enabled)
        {
            leftSelectAction.Enable();
            Debug.Log("[RayController2] 좌측 Input Action 재활성화");
        }
        if (rightSelectAction != null && !rightSelectAction.enabled)
        {
            rightSelectAction.Enable();
            Debug.Log("[RayController2] 우측 Input Action 재활성화");
        }
    }
}
