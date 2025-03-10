using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("좌측 Select: XRI LeftHand Interaction/Select")]
    public string leftSelectActionName = "XRI LeftHand Interaction/Select";

    [Header("우측 Select: XRI RightHand Interaction/Select")]
    public string rightSelectActionName = "XRI RightHand Interaction/Select";

    [Header("디버그 (OnButtonClick 호출 여부)")]
    public bool buttonClicked = false;

    // 좌측 및 우측 Select 액션 저장
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 레이가 버튼 위에 있는지 여부
    private bool isRayHovering = false;

    void Start()
    {
        if (button == null)
        {
            Debug.LogError("[TestButton2] Button이 할당되지 않았습니다.");
        }
    }

    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 액션 구독 및 활성화
            leftSelectAction = inputActionAsset.FindAction(leftSelectActionName, true);
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += OnSelectPerformed;
                leftSelectAction.Enable();
            }
            else
            {
                Debug.LogError("[TestButton2] '" + leftSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }

            // 우측 액션 구독 및 활성화
            rightSelectAction = inputActionAsset.FindAction(rightSelectActionName, true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectPerformed;
                rightSelectAction.Enable();
            }
            else
            {
                Debug.LogError("[TestButton2] '" + rightSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestButton2] InputActionAsset이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

    // 이벤트 구독
    void OnDisable()
    {
        if (leftSelectAction != null)
        {
            leftSelectAction.performed -= OnSelectPerformed;
            leftSelectAction.Disable();
        }
        if (rightSelectAction != null)
        {
            rightSelectAction.performed -= OnSelectPerformed;
            rightSelectAction.Disable();
        }
    }

    // 버튼 위에 레이가 있을 때 XR 컨트롤러의 Grip 입력이 발생하면 호출
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (isRayHovering)
        {
            // XR 입력 시 버튼 애니메이션 효과를 시뮬레이션하고 클릭 이벤트 호출
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
    }

    // 버튼 위에 레이가 진입 시 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        isRayHovering = true;
    }

    // 버튼 영역을 벗어나면 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        isRayHovering = false;
    }

    // XR 입력 시 버튼 애니메이션 및 클릭 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("[TestButton2] EventSystem.current가 null입니다.");
            yield break;
        }

        // 포인터 이벤트 데이터 생성
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerPress = button.gameObject
        };

        // 버튼 누름 효과 시작
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 애니메이션 효과를 위해 잠시 대기 (0.1초)
        yield return new WaitForSeconds(0.1f);

        // 버튼 누름 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 클릭 이벤트 호출
        OnButtonClick();
    }

    // 버튼 클릭 처리
    public void OnButtonClick()
    {
        buttonClicked = true;
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
