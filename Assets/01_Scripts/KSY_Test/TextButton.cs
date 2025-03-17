//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
//using System.Collections;
//using TMPro;

//public class TextButton : MonoBehaviour
//{
//    [Header("UI")]
//    public Button button;
//    public TextMeshProUGUI displayText;

//    [Header("XRI Default Input Actions")]
//    public InputActionAsset inputActionAsset;

//    // 좌측, 우측 Select 액션
//    private InputAction leftSelectAction;
//    private InputAction rightSelectAction;

//    // 텍스트 원본 저장 및 복원 코루틴 관리
//    private string originalText;
//    private Coroutine resetCoroutine;

//    // 현재 버튼 위에 있는 레이 상태 
//    private bool leftHover = false;
//    private bool rightHover = false;

//    void Start()
//    {
//        if (displayText != null)
//            originalText = displayText.text;
//    }

//    void OnEnable()
//    {
//        if (inputActionAsset != null)
//        {
//            // 좌측 컨트롤러 Select 액션 구독 및 활성화
//            leftSelectAction = inputActionAsset.FindAction("XRI LeftHand Interaction/Select", true);
//            if (leftSelectAction != null)
//            {
//                leftSelectAction.performed += context => ProcessSelectPerformed(context, "left");
//                leftSelectAction.Enable();
//                Debug.Log("[TextButton] 좌측 컨트롤러 Select 액션 구독 및 활성화");
//            }
//            else
//            {
//                Debug.LogError("[TextButton] 좌측 컨트롤러 Select 액션을 찾을 수 없습니다.");
//            }

//            // 우측 컨트롤러 Select 액션 구독 및 활성화
//            rightSelectAction = inputActionAsset.FindAction("XRI RightHand Interaction/Select", true);
//            if (rightSelectAction != null)
//            {
//                rightSelectAction.performed += context => ProcessSelectPerformed(context, "right");
//                rightSelectAction.Enable();
//                Debug.Log("[TextButton] 우측 컨트롤러 Select 액션 구독 및 활성화");
//            }
//            else
//            {
//                Debug.LogError("[TextButton] 우측 컨트롤러 Select 액션을 찾을 수 없습니다.");
//            }
//        }
//        else
//        {
//            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다.");
//        }
//    }

//    void OnDisable()
//    {
//        if (leftSelectAction != null)
//        {
//            leftSelectAction.performed -= context => ProcessSelectPerformed(context, "left");
//            leftSelectAction.Disable();
//        }
//        if (rightSelectAction != null)
//        {
//            rightSelectAction.performed -= context => ProcessSelectPerformed(context, "right");
//            rightSelectAction.Disable();
//        }
//    }

//    // XR Ray Interactor -> UI Hover Entered 및 Exited에서 호출 (왼쪽)
//    // UI Hover Entered -> true 체크
//    public void SetLeftHover(bool isHover)
//    {
//        SetHover("left", isHover);
//    }

//    // XR Ray Interactor -> UI Hover Entered 및 Exited에서 호출 (오른쪽)
//    // UI Hover Entered -> true 체크
//    public void SetRightHover(bool isHover)
//    {
//        SetHover("right", isHover);
//    }

//    // XR Ray Interactor -> UI Hover Entered 및 Exited에서
//    // 호출되어 버튼 위에 레이가 있는 상태를 업데이트
//    private void SetHover(string hand, bool isHover)
//    {
//        if (hand.ToLower() == "left")
//        {
//            leftHover = isHover;
//            Debug.Log($"[TextButton] 좌측 컨트롤러 레이 여부 {isHover}");
//        }
//        else if (hand.ToLower() == "right")
//        {
//            rightHover = isHover;
//            Debug.Log($"[TextButton] 우측 컨트롤러 레이 {isHover}");
//        }
//    }

//    // 버튼 위에 레이가 있을 경우에만 애니메이션 및 클릭 실행
//    private void ProcessSelectPerformed(InputAction.CallbackContext context, string hand)
//    {
//        bool isHover = (hand.ToLower() == "left") ? leftHover : rightHover;
//        if (isHover)
//        {
//            StartCoroutine(TriggerButtonAnimationAndClick(hand));
//        }
//        else
//        {
//            Debug.Log($"[TextButton] {hand} 컨트롤러 Select 입력은 감지되었으나, 레이가 버튼 위에 있지 않음");
//        }
//    }

//    // 버튼 클릭 애니메이션과 이벤트 처리
//    private IEnumerator TriggerButtonAnimationAndClick(string hand)
//    {
//        if (EventSystem.current == null)
//        {
//            Debug.LogError("[TextButton] EventSystem.current가 null입니다.");
//            yield break;
//        }

//        PointerEventData pointerData = new PointerEventData(EventSystem.current)
//        {
//            pointerPress = button.gameObject
//        };

//        // 버튼 효과 시작
//        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

//        // 버튼 효과 지속
//        yield return new WaitForSeconds(0.1f);

//        // 버튼 효과 종료
//        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

//        OnButtonClickForHand(hand);
//    }

//    // 버튼 클릭 처리
//    public void OnButtonClickForHand(string hand)
//    {
//        Debug.Log($"[TextButton] {hand} 컨트롤러 버튼 클릭 처리 완료");
//        if (displayText != null)
//            displayText.text = $"{hand} 컨트롤러 버튼 클릭 처리 완료";

//        ResetTextAfterDelay();
//    }

//    // 텍스트를 원래 상태로 복원하기 위한 딜레이
//    public void ResetTextAfterDelay()
//    {
//        // 이전에 실행 중인 딜레이 복원 코루틴이 있다면 중지
//        if (resetCoroutine != null)
//            StopCoroutine(resetCoroutine);

//        // 새로운 딜레이 복원 코루틴 시작
//        resetCoroutine = StartCoroutine(ResetTextCoroutine());
//    }

//    // 일정 시간 이후에 텍스트 복원
//    private IEnumerator ResetTextCoroutine()
//    {
//        // 2초 동안 대기
//        yield return new WaitForSeconds(2f);

//        // 텍스트를 원래 상태로 복원
//        if (displayText != null)
//            displayText.text = originalText;
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInputData
{
    [Header("플레이어 포인터 ID 정보")]
    public PlayerPointerId playerPointerId;
}

public enum ButtonTypeA
{
    A,
    B
}

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Button button;

    [Header("버튼 식별")]
    public ButtonTypeA buttonType;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // 여러 플레이어의 입력 정보 관리
    [Header("플레이어 입력 정보")]
    public List<PlayerInputData> playerInputDataList;

    // Select 액션 (좌측, 우측 컨트롤러)
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // Pointer 이벤트로 레이의 진입 상태를 관리
    private bool isHovered = false;

    // 현재 버튼 위에 있는 포인터의 ID (여러 포인터가 동시에 있다면 첫번째를 기준으로 함)
    private int currentPointerId = -1;

    private void Awake()
    {
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
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnEnable()
    {
        if (inputActionAsset != null)
        {
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

    // IPointerEnterHandler 구현
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        currentPointerId = eventData.pointerId;

        bool isLeft = false;
        bool isRight = false;

        // playerInputDataList를 확인하여 해당 포인터가 어느 플레이어의 좌/우 포인터인지 판별
        if (playerInputDataList != null)
        {
            foreach (PlayerInputData inputData in playerInputDataList)
            {
                // 특정 플레이어만 처리하려면 아래처럼 userId 비교로 분기 가능(예시)
                // if (inputData.playerPointerId.userId != 1) continue;

                // 좌측 포인터 ID 검사
                foreach (PlayerPointerId.LeftPointerId leftId in inputData.playerPointerId.leftPointerIds)
                {
                    if ((int)leftId == currentPointerId)
                    {
                        isLeft = true;
                        break;
                    }
                }
                // 우측 포인터 ID 검사
                foreach (PlayerPointerId.RightPointerId rightId in inputData.playerPointerId.rightPointerIds)
                {
                    if ((int)rightId == currentPointerId)
                    {
                        isRight = true;
                        break;
                    }
                }
            }
        }

        if (isLeft)
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 좌측 포인터({currentPointerId}) 진입");
        }
        else if (isRight)
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 우측 포인터({currentPointerId}) 진입");
        }
        else
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 기타 포인터({currentPointerId}) 진입");
        }
    }

    // IPointerExitHandler 구현
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        int pointerId = eventData.pointerId;
        bool isLeft = false;
        bool isRight = false;

        if (playerInputDataList != null)
        {
            foreach (PlayerInputData inputData in playerInputDataList)
            {
                foreach (PlayerPointerId.LeftPointerId leftId in inputData.playerPointerId.leftPointerIds)
                {
                    if ((int)leftId == pointerId)
                    {
                        isLeft = true;
                        break;
                    }
                }
                foreach (PlayerPointerId.RightPointerId rightId in inputData.playerPointerId.rightPointerIds)
                {
                    if ((int)rightId == pointerId)
                    {
                        isRight = true;
                        break;
                    }
                }
            }
        }

        if (isLeft)
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 좌측 포인터({pointerId}) 이탈");
        }
        else if (isRight)
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 우측 포인터({pointerId}) 이탈");
        }
        else
        {
            Debug.Log($"[TestButton2] {buttonType} 버튼 - 기타 포인터({pointerId}) 이탈");
        }

        currentPointerId = -1;
    }

    // 좌/우 컨트롤러의 Select 액션 공통 처리
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        ProcessSelectPerformed();
    }

    private void ProcessSelectPerformed()
    {
        if (isHovered)
        {
            bool isLeft = false;
            bool isRight = false;

            if (playerInputDataList != null)
            {
                foreach (PlayerInputData inputData in playerInputDataList)
                {
                    // 특정 플레이어만 처리하려면 아래처럼 userId 비교로 분기 가능(예시)
                    // if (inputData.playerPointerId.userId != 1) continue;

                    foreach (PlayerPointerId.LeftPointerId leftId in inputData.playerPointerId.leftPointerIds)
                    {
                        if ((int)leftId == currentPointerId)
                        {
                            isLeft = true;
                            break;
                        }
                    }
                    foreach (PlayerPointerId.RightPointerId rightId in inputData.playerPointerId.rightPointerIds)
                    {
                        if ((int)rightId == currentPointerId)
                        {
                            isRight = true;
                            break;
                        }
                    }
                }
            }

            if (isLeft)
            {
                Debug.Log($"[TestButton2] 좌측 버튼 입력으로 이벤트 처리 (포인터 ID: {currentPointerId})");
                // 좌측 입력 전용 처리 로직 추가 가능
            }
            else if (isRight)
            {
                Debug.Log($"[TestButton2] 우측 버튼 입력으로 이벤트 처리 (포인터 ID: {currentPointerId})");
                // 우측 입력 전용 처리 로직 추가 가능
            }
            else
            {
                Debug.Log($"[TestButton2] 일반 버튼 입력으로 이벤트 처리 (포인터 ID: {currentPointerId})");
            }

            StartCoroutine(TriggerButtonAnimationAndClick());
        }
        else
        {
            Debug.Log($"[TestButton2] Select 입력은 감지되었으나, {buttonType} 버튼 위에 레이가 없음");
        }
    }

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

        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(0.1f);
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
        button.onClick.Invoke();
    }

    private void OnButtonClicked()
    {
        switch (buttonType)
        {
            case ButtonTypeA.A:
                Debug.Log("스위치문: 버튼 A 클릭 이벤트 발생");
                break;
            case ButtonTypeA.B:
                Debug.Log("스위치문: 버튼 B 클릭 이벤트 발생");
                break;
            default:
                Debug.Log("스위치문: 미지정 버튼 클릭 이벤트 발생");
                break;
        }
    }
}
