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

    [Header("XRI Default Input Actions 내에서 Select(Grip) 액션의 이름")]
    public string selectActionName = "Select";

    private InputAction selectAction;
    private string originalText;
    private Coroutine resetCoroutine;
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

        if (displayText = null)
        {
            Debug.LogError("[TextButton] displayText가 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            selectAction = inputActionAsset.FindAction(selectActionName, true);
            if (selectAction != null)
            {
                selectAction.performed += OnSelectPerformed;
                selectAction.Enable();
                Debug.Log("[TextButton] '" + selectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TextButton] '" + selectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

    void OnDisable()
    {
        if (selectAction != null)
        {
            selectAction.performed -= OnSelectPerformed;
            selectAction.Disable();
        }
    }

    // XR 입력(그립 버튼) 수행 시 호출
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[TextButton] Select(Grip) 액션 performed. isRayHovering: " + isRayHovering);
        if (isRayHovering)
        {
            Debug.Log("[TextButton] Grip 버튼 입력 및 Ray Hovering 상태, 버튼 클릭 처리");

            // XR 입력 시 버튼 애니메이션 효과를 시뮬레이션 후 클릭 처리
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
    }

    // 버튼 위에 Ray가 진입 시 호출
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

    // XR 입력 시 버튼 애니메이션 및 클릭 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        // 포인터 이벤트 데이터 생성 
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerPress = button.gameObject
        };

        // pointer down 이벤트 시뮬레이션 (버튼 눌림 애니메이션 시작)
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 눌림 효과를 보여주기 위해 잠시 대기 (예: 0.1초)
        yield return new WaitForSeconds(0.1f);

        // pointer up 이벤트 시뮬레이션 (버튼 눌림 애니메이션 종료)
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 클릭 처리
        OnButtonClick();
    }

    // XR 입력 시 호출
    public void OnButtonClick()
    {
        buttonClicked = true;
        Debug.Log("[TextButton] OnButtonClick() 호출됨 (XR 입력에 의한 호출).");

        if (displayText != null)
        {
            displayText.text = "버튼 클릭 처리 완료!";
            Debug.Log("[TextButton] displayText 업데이트: " + displayText.text);

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetTextAndButtonCoroutine());
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 null입니다. 텍스트 변경 불가.");
        }
    }

    // 2초 후 원래 텍스트로 복원하는 코루틴
    IEnumerator ResetTextAndButtonCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (displayText != null)
        {
            displayText.text = originalText;
            Debug.Log("[TextButton] displayText 원래 텍스트로 복원됨: " + originalText);
        }
        buttonClicked = false;
        Debug.Log("[TextButton] buttonClicked 값이 false로 설정됨.");
        resetCoroutine = null;
    }
}
