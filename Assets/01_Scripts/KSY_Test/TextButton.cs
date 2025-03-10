using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Button button;
    public TextMeshProUGUI displayText;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("좌측 Select: XRI LeftHand Interaction/Select")]
    public string leftSelectActionName = "XRI LeftHand Interaction/Select";

    [Header("우측 Select: XRI RightHand Interaction/Select")]
    public string rightSelectActionName = "XRI RightHand Interaction/Select";

    [Header("디버그 (호출 여부)")]
    public bool buttonClicked = false;

    // 좌측 및 우측 Select 액션 저장
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 텍스트 기본값 -> 텍스트 복원 시 사용
    private string originalText;

    // 텍스트 복원 코루틴의 중복 실행을 방지하기 위해 사용
    private Coroutine resetCoroutine;

    // 좌측, 우측 레이가 버튼 위에 있는지 여부
    private bool isLeftRayHovering = false;
    private bool isRightRayHovering = false;

    void Start()
    {
        if (button == null)
        {
            Debug.LogError("[TextButton] Button이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }

        if (displayText == null)
        {
            Debug.LogError("[TextButton] displayText가 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
        else
        {
            // 원래 텍스트 값을 저장
            originalText = displayText.text;
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
                Debug.Log("[TextButton] '" + leftSelectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TextButton] '" + leftSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }

            // 우측 액션 구독 및 활성화
            rightSelectAction = inputActionAsset.FindAction(rightSelectActionName, true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectPerformed;
                rightSelectAction.Enable();
                Debug.Log("[TextButton] '" + rightSelectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TextButton] '" + rightSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

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

    // XR 입력 발생 시, 해당 컨트롤러의 레이가 버튼 위에 있는지 확인 후 클릭 이벤트 처리
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        // 좌측 컨트롤러 입력 && 좌측 레이가 버튼 위에 있을 경우 처리
        if (context.action == leftSelectAction && isLeftRayHovering)
        {
            Debug.Log("[TextButton] 좌측 컨트롤러 입력 감지, 좌측 레이가 버튼 위에 있음.");
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
        // 우측 컨트롤러 입력 && 우측 레이가 버튼 위에 있을 경우 처리
        else if (context.action == rightSelectAction && isRightRayHovering)
        {
            Debug.Log("[TextButton] 우측 컨트롤러 입력 감지, 우측 레이가 버튼 위에 있음.");
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
        else
        {
            Debug.Log("[TextButton] 해당 컨트롤러의 레이가 버튼 위에 있지 않음.");
        }
    }

    // 버튼 위에 레이가 진입하면, PointerEventData.pointerId를 통해 좌측/우측 구분
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[TextButton] Pointer Enter: pointerId " + eventData.pointerId);

        // 좌측: pointerId 2 또는 9
        if (eventData.pointerId == 2 || eventData.pointerId == 9)
        {
            isLeftRayHovering = true;
            Debug.Log("[TextButton] 좌측 레이 진입");
        }
        // 우측: pointerId 5 또는 3
        else if (eventData.pointerId == 5 || eventData.pointerId == 3)
        {
            isRightRayHovering = true;
            Debug.Log("[TextButton] 우측 레이 진입");
        }
    }

    // 버튼 영역을 벗어나면 해당 컨트롤러의 hovering 상태 업데이트
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("[TextButton] Pointer Exit: pointerId " + eventData.pointerId);

        if (eventData.pointerId == 2 || eventData.pointerId == 9)
        {
            isLeftRayHovering = false;
            Debug.Log("[TextButton] 좌측 레이 벗어남");
        }
        else if (eventData.pointerId == 5 || eventData.pointerId == 3)
        {
            isRightRayHovering = false;
            Debug.Log("[TextButton] 우측 레이 벗어남");
        }

        // 두 레이 모두 버튼 위에 없으면 원래 텍스트 복원 코루틴 실행
        if (!isLeftRayHovering && !isRightRayHovering)
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetTextAndButtonCoroutine());
        }
    }

    // XR 입력 시 버튼 애니메이션 및 클릭 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("[TextButton] EventSystem.current가 null입니다.");
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
        Debug.Log("[TextButton] OnButtonClick() 호출됨.");

        if (displayText != null)
        {
            displayText.text = "버튼 클릭 처리 완료!";

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetTextAndButtonCoroutine());
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 null입니다.");
        }
    }

    // 일정 시간 후 원래 텍스트로 복원하는 코루틴
    IEnumerator ResetTextAndButtonCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (displayText != null)
        {
            displayText.text = originalText;
        }

        buttonClicked = false;
        resetCoroutine = null;
    }
}
