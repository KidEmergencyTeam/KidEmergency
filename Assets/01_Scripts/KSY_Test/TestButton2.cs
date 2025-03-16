using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class TestButton2 : MonoBehaviour
{
    [Header("UI")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // 좌측, 우측 Select 액션
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 현재 버튼 위에 있는 레이 상태 
    private bool leftHover = false;
    private bool rightHover = false;

    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 컨트롤러 Select 액션 구독 및 활성화
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
            if (leftSelectAction != null)
            {
                // 람다 대신 메서드 참조(이벤트 메서드)로 등록
                leftSelectAction.performed += OnLeftSelectActionPerformed;
                leftSelectAction.Enable();
                Debug.Log("[TextButton] 좌측 컨트롤러 Select 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError("[TextButton] 좌측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }

            // 우측 컨트롤러 Select 액션 구독 및 활성화
            rightSelectAction = inputActionAsset.FindAction("XRI RightHand Interaction/Select", true);
            if (rightSelectAction != null)
            {
                // 람다 대신 메서드 참조(이벤트 메서드)로 등록
                rightSelectAction.performed += OnRightSelectActionPerformed;
                rightSelectAction.Enable();
                Debug.Log("[TextButton] 우측 컨트롤러 Select 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError("[TextButton] 우측 컨트롤러 Select 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다.");
        }
    }

    void OnDisable()
    {
        if (leftSelectAction != null)
        {
            // 등록했던 메서드 참조를 동일하게 이용해 해제
            leftSelectAction.performed -= OnLeftSelectActionPerformed;
            leftSelectAction.Disable();
        }
        if (rightSelectAction != null)
        {
            // 등록했던 메서드 참조를 동일하게 이용해 해제
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

    // 버튼 위의 레이 상태 업데이트
    private void SetHover(string hand, bool isHover)
    {
        if (hand.ToLower() == "left")
        {
            leftHover = isHover;
            Debug.Log($"[TextButton] 좌측 컨트롤러 레이 여부 {isHover}");
        }
        else if (hand.ToLower() == "right")
        {
            rightHover = isHover;
            Debug.Log($"[TextButton] 우측 컨트롤러 레이 {isHover}");
        }
    }

    // 좌측 컨트롤러 Select 이벤트 메서드
    private void OnLeftSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed(context, "left");
    }

    // 우측 컨트롤러 Select 이벤트 메서드
    private void OnRightSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed(context, "right");
    }

    // 좌측, 우측 입력 처리
    private void ProcessSelectPerformed(InputAction.CallbackContext context, string hand)
    {
        bool isHover = (hand.ToLower() == "left") ? leftHover : rightHover;
        if (isHover)
        {
            StartCoroutine(TriggerButtonAnimationAndClick(hand));
        }
        else
        {
            Debug.Log($"[TextButton] {hand} 컨트롤러 Select 입력은 감지되었으나, 레이가 버튼 위에 있지 않음");
        }
    }

    // 버튼 클릭 애니메이션과 이벤트 처리
    private IEnumerator TriggerButtonAnimationAndClick(string hand)
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("[TextButton] EventSystem.current가 null입니다.");
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

        OnButtonClickForHand(hand);
    }

    // 버튼 클릭 처리
    public void OnButtonClickForHand(string hand)
    {
        Debug.Log($"[TextButton] {hand} 컨트롤러 버튼 클릭 처리 완료");
    }
}
