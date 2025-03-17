using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public enum ButtonType
{
    A,
    B
}

public class TestButton2 : MonoBehaviour
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

    // 각 컨트롤러의 레이 상태
    private bool leftHover = false;
    private bool rightHover = false;

    // 현재 hover 중인 버튼을 추적
    private static TestButton2 currentHoveredLeft = null;
    private static TestButton2 currentHoveredRight = null;

    private void Awake()
    {
        // onClick 이벤트 등록
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("[TestButton2] Button 컴포넌트가 할당되지 않았습니다.");
        }
    }

    // 중간에 버튼 null 상황일 때, 버튼 이벤트 제거
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 컨트롤러 Select 액션 설정
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += OnLeftSelectActionPerformed;
                leftSelectAction.Enable();
                Debug.Log("[TestButton2] 좌측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("[TestButton2] 좌측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }

            // 우측 컨트롤러 Select 액션 설정
            rightSelectAction = inputActionAsset.FindAction("XRI RightHand Interaction/Select", true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnRightSelectActionPerformed;
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
            leftSelectAction.performed -= OnLeftSelectActionPerformed;
            leftSelectAction.Disable();
        }
        if (rightSelectAction != null)
        {
            rightSelectAction.performed -= OnRightSelectActionPerformed;
            rightSelectAction.Disable();
        }
    }

    // XR Ray Interactor -> UI Hover Entered 및 Exited에서 호출 (왼쪽)
    // UI Hover Entered -> true 체크
    public void SetLeftHover(bool isHover)
    {
        SetHover("left", isHover);
    }

    // XR Ray Interactor -> UI Hover Entered 및 Exited에서 호출 (오른쪽)
    // UI Hover Entered -> true 체크
    public void SetRightHover(bool isHover)
    {
        SetHover("right", isHover);
    }

    // XR Ray Interactor -> UI Hover Entered 및 Exited에서
    // 호출되어 버튼 위에 레이가 있는 상태를 업데이트
    private void SetHover(string hand, bool isHover)
    {
        string buttonIdentifier = buttonType.ToString();

        if (hand.ToLower() == "left")
        {
            if (isHover)
            {
                if (currentHoveredLeft != null && currentHoveredLeft != this)
                {
                    Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 좌측 컨트롤러: 이미 {currentHoveredLeft.buttonType} 버튼이 hover 중.");
                    return;
                }
                leftHover = true;
                currentHoveredLeft = this;
                Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 좌측 컨트롤러 레이 상태: {isHover}");
            }
            else
            {
                if (currentHoveredLeft == this)
                {
                    leftHover = false;
                    currentHoveredLeft = null;
                    Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 좌측 컨트롤러 레이 상태: {isHover}");
                }
            }
        }
        else if (hand.ToLower() == "right")
        {
            if (isHover)
            {
                if (currentHoveredRight != null && currentHoveredRight != this)
                {
                    Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 우측 컨트롤러: 이미 {currentHoveredRight.buttonType} 버튼이 hover 중.");
                    return;
                }
                rightHover = true;
                currentHoveredRight = this;
                Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 우측 컨트롤러 레이 상태: {isHover}");
            }
            else
            {
                if (currentHoveredRight == this)
                {
                    rightHover = false;
                    currentHoveredRight = null;
                    Debug.Log($"[TestButton2] {buttonIdentifier} 버튼 - 우측 컨트롤러 레이 상태: {isHover}");
                }
            }
        }
    }

    // 좌측 컨트롤러 입력 처리
    private void OnLeftSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed("left");
    }

    // 우측 컨트롤러 입력 처리
    private void OnRightSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed("right");
    }

    // 버튼 위에 레이가 있을 경우에만 애니메이션 및 클릭 실행
    private void ProcessSelectPerformed(string hand)
    {
        bool isHover = (hand.ToLower() == "left") ? leftHover : rightHover;
        if (isHover)
        {
            StartCoroutine(TriggerButtonAnimationAndClick(hand));
        }
        else
        {
            Debug.Log($"[TestButton2] {hand} 컨트롤러의 Select 입력은 감지되었으나, {buttonType} 버튼 위에 레이가 없음");
        }
    }

    // 버튼 클릭 애니메이션과 이벤트 처리
    private IEnumerator TriggerButtonAnimationAndClick(string hand)
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

        // 버튼 효과 시작 
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 버튼 효과 지속 
        yield return new WaitForSeconds(0.1f);

        // 버튼 효과 종료 
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 이벤트 즉시 실행
        button.onClick.Invoke();
    }

    // 스위치문을 이용하여 버튼 타입별 이벤트 분기 처리
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
            default:
                Debug.Log("스위치문: 미지정 버튼 클릭 이벤트 발생");
                break;
        }
    }
}
