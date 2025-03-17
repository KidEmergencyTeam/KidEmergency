using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class TextButton : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public TextMeshProUGUI displayText;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // 좌측, 우측 Select 액션
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 텍스트 원본 저장 및 복원 코루틴 관리
    private string originalText;
    private Coroutine resetCoroutine;

    // 현재 버튼 위에 있는 레이 상태 
    private bool leftHover = false;
    private bool rightHover = false;

    void Start()
    {
        if (displayText != null)
            originalText = displayText.text;
    }

    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 컨트롤러 Select 액션 구독 및 활성화
            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += context => ProcessSelectPerformed(context, "left");
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
                rightSelectAction.performed += context => ProcessSelectPerformed(context, "right");
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
            leftSelectAction.performed -= context => ProcessSelectPerformed(context, "left");
            leftSelectAction.Disable();
        }
        if (rightSelectAction != null)
        {
            rightSelectAction.performed -= context => ProcessSelectPerformed(context, "right");
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

    // 버튼 위에 레이가 있을 경우에만 애니메이션 및 클릭 실행
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
        if (displayText != null)
            displayText.text = $"{hand} 컨트롤러 버튼 클릭 처리 완료";

        ResetTextAfterDelay();
    }

    // 텍스트를 원래 상태로 복원하기 위한 딜레이
    public void ResetTextAfterDelay()
    {
        // 이전에 실행 중인 딜레이 복원 코루틴이 있다면 중지
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        // 새로운 딜레이 복원 코루틴 시작
        resetCoroutine = StartCoroutine(ResetTextCoroutine());
    }

    // 일정 시간 이후에 텍스트 복원
    private IEnumerator ResetTextCoroutine()
    {
        // 2초 동안 대기
        yield return new WaitForSeconds(2f);

        // 텍스트를 원래 상태로 복원
        if (displayText != null)
            displayText.text = originalText;
    }
}
