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

    [Header("Grabber")]
    [SerializeField] private Grabber leftGrabber;

    // 현재 활성화된 레이가 오른손인지 여부 (true: 오른손, false: 좌측)
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
            // 좌측 그립 액션 찾기
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
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

            // 우측 그립 액션 찾기
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

    // 해당 객체가 비활성화 및 제거될 때 또는 씬 전환 시 호출하여 이벤트를 해제한다.
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

    // 그립 입력에 따라 좌측 레이 활성화 및 우측 레이 비활성화, 우측 레이 활성화 및 좌측 레이 비활성화 처리
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        // 좌측 그립 입력 시
        if (context.action == leftSelectAction)
        {
            if (isRightActive)
            {
                SwitchLeftRay();
            }
        }
        // 우측 그립 입력 시
        else if (context.action == rightSelectAction)
        {
            if (!isRightActive)
            {
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

        Debug.Log("[RayController2] 좌측 레이 활성화");
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

        Debug.Log("[RayController2] 우측 레이 활성화");
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
