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

    // 그립 입력 처리 시, 좌측 Grabber의 Grabbed 상태를 확인하여 오브젝트가 잡힌 경우에는 레이 전환을 막고 우측 레이를 고정
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        // 좌측 Grabber에서 오브젝트를 잡고 있다면
        if (leftGrabber != null && leftGrabber.Grabbed)
        {
            // 우측 레이가 활성화되지 않았다면 강제로 전환
            if (!isRightActive)
            {
                SwitchRightRay();
            }
            Debug.Log("[RayController2] 오브젝트가 잡힌 상태이므로 레이 전환 불가");
            return;
        }

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
