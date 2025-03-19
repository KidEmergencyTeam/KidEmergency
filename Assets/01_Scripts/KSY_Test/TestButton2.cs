using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

// TestButton2.cs 목적: 할당된 버튼 이벤트 출력
public enum ButtonType
{
    A,
    B,
    C,
    D,
    E,
    F
}

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Button button;

    [Header("버튼 식별")]
    public ButtonType buttonType;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // 좌측, 우측 컨트롤러의 Select 액션
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // Pointer 이벤트로 레이의 진입 상태를 관리
    private bool isHovered = false;

    // 버튼 클릭 상태를 관리
    public bool isClick = false;

    private void Awake()
    {
        // 버튼 이벤트 할당
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("[TestButton2] Button 컴포넌트가 할당되지 않았습니다.");
        }
    }

    private void OnDestroy()
    {
        // 중간에 버튼이 null 상황일 때, 이벤트 제거
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 컨트롤러 Select 액션 구독 및 활성화
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += OnSelectActionPerformed;
                leftSelectAction.Enable();
                Debug.Log("[TestButton2] 좌측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("[TestButton2] 좌측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }

            // 우측 컨트롤러 Select 액션 구독 및 활성화
            rightSelectAction = inputActionAsset.FindAction("XRI RightHand Interaction/Select", true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectActionPerformed;
                rightSelectAction.Enable();
                Debug.Log("[TestButton2] 우측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("[TestButton2] 우측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestButton2] InputActionAsset이 할당되지 않았습니다.");
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

    // IPointerEnterHandler 구현: 레이가 버튼 영역에 있을때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 레이가 버튼 영역에 있을때
        // isHovered 플래그를 true로 설정
        isHovered = true;

        // 어느 버튼에서 레이가 진입했는지 확인
        Debug.Log($"[TestButton2] {buttonType} 버튼 - Pointer Enter");
    }

    // IPointerExitHandler 구현: 레이가 버튼 영역에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 레이가 버튼 영역에서 벗어났을 때
        // isHovered 플래그를 false로 설정
        isHovered = false;

        // 어느 버튼에서 레이가 벗어났는지 확인
        Debug.Log($"[TestButton2] {buttonType} 버튼 - Pointer Exit");
    }

    // 좌측/우측 컨트롤러의 Select 액션 이벤트 처리 (두 액션 모두 동일하게 처리)
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed();
    }

    // 해당 버튼이 레이가 진입 상태일 경우에만 클릭 처리
    private void ProcessSelectPerformed()
    {
        if (isHovered)
        {
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
        else
        {
            Debug.Log($"[TestButton2] Select 입력은 감지되었으나, {buttonType} 버튼 위에 레이가 없음");
        }
    }

    // 버튼 눌림 효과 및 이벤트 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("[TestButton2] EventSystem.current가 null입니다.");
            yield break;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerPress = button.gameObject
        };

        // 버튼 눌림 효과 시작
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 버튼 눌림 효과 지속 시간
        yield return new WaitForSeconds(0.1f);

        // 버튼 눌림 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 이벤트 실행
        button.onClick.Invoke();

        // 버튼 클릭 했을때 
        // isClick 플래그를 true로 설정
        isClick = true;
    }

    // 버튼 클릭 이벤트 처리 (스위치문으로 분기) -> 단순 상태만 표시
    private void OnButtonClicked()
    {
        switch (buttonType)
        {
            case ButtonType.A:
                Debug.Log("스위치문: 버튼 A 클릭 이벤트 발생");
                break;
            case ButtonType.B:
                Debug.Log("스위치문: 버튼 B 클릭 이벤트 발생");
                break;
            case ButtonType.C:
                Debug.Log("스위치문: 버튼 C 클릭 이벤트 발생");
                break;
            case ButtonType.D:
                Debug.Log("스위치문: 버튼 D 클릭 이벤트 발생");
                break;
            case ButtonType.E:
                Debug.Log("스위치문: 버튼 E 클릭 이벤트 발생");
                break;
            case ButtonType.F:
                Debug.Log("스위치문: 버튼 F 클릭 이벤트 발생");
                break;
            default:
                Debug.Log("스위치문: 미지정 버튼 클릭 이벤트 발생");
                break;
        }
    }
}
