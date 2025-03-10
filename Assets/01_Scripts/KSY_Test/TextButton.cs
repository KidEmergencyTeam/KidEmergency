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

    // 좌측 및 우측 Select 액션 저장
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 텍스트 기본값 -> 텍스트 기본값 복원할 때 사용
    private string originalText;

    // 텍스트 복원 코루틴의 중복 실행을 방지하기 위해 사용
    private Coroutine resetCoroutine;

    // 레이가 버튼 위에 있는지 여부
    private bool isRayHovering = false;

    [Header("디버그 (호출 여부)")]
    [SerializeField]
    private bool buttonClicked = false;

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

    // 버튼 위에 Ray가 진입할 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        isRayHovering = true;
        Debug.Log("[TextButton] Ray가 버튼 위에 진입함: " + eventData.pointerEnter);
    }

    // 버튼 영역을 벗어나면 일정 시간 후 원래 텍스트로 복원
    public void OnPointerExit(PointerEventData eventData)
    {
        isRayHovering = false;
        Debug.Log("[TextButton] Ray가 버튼 영역에서 벗어남: " + eventData.pointerEnter);

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(ResetTextAndButtonCoroutine());
    }

    // XR 입력 시 버튼 애니메이션 및 클릭 효과 시뮬레이션
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        // 포인터 이벤트 데이터 생성
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerPress = button.gameObject
        };

        // 버튼 누름 효과 시작
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(0.1f);
        // 버튼 누름 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

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
