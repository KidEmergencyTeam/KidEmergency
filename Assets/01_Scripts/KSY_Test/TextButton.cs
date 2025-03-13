using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Select 액션 타입 열거형 정의
public enum XRActionType
{
    LeftHandInteractionSelect,
    RightHandInteractionSelect
}

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Button button;
    public TextMeshProUGUI displayText;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("Select 액션 타입")]
    public XRActionType leftSelectActionType = XRActionType.LeftHandInteractionSelect;
    public XRActionType rightSelectActionType = XRActionType.RightHandInteractionSelect;

    [Header("포인터 ID 관리 스크립트")]
    public List<PlayerPointerId> pointerIds;

    // 좌측 및 우측 Select 액션
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 각 플레이어별로 버튼 위에 레이가 위치했는지 여부 저장
    private Dictionary<int, bool> leftHoverStates = new Dictionary<int, bool>();
    private Dictionary<int, bool> rightHoverStates = new Dictionary<int, bool>();

    // 동시 입력 방지를 위한 클릭 처리 플래그
    private Dictionary<int, bool> isProcessingClick = new Dictionary<int, bool>();

    // 포인터ID -> (userId, isLeft) 매핑 (반복문 최적화)
    private Dictionary<int, (int userId, bool isLeft)> pointerMapping = new Dictionary<int, (int, bool)>();

    // 텍스트 복원 코루틴 중복 실행 방지
    private string originalText;
    private Coroutine resetCoroutine;

    // 플레이어 검색 주기 (초)
    private float checkInterval = 1f;
    private float lastCheckTime = 0f;

    void Start()
    {
        // pointerIds가 null 상태인 경우를 명시적으로 피하기 위해 새로운 빈 리스트를 생성
        // null 체크시 발생할 수 있는 NullReferenceException 오류 방지
        if (pointerIds == null)
            pointerIds = new List<PlayerPointerId>();

        // displayText의 원래 텍스트 저장
        if (displayText != null)
            originalText = displayText.text;
    }

    void Update()
    {
        // 일정 시간 간격으로 플레이어 목록 갱신
        if (Time.time - lastCheckTime > checkInterval)
        {
            lastCheckTime = Time.time;

            // 1. pointerIds 리스트에 할당된 pointer.cs가 null 상태이거나
            // 해당 플레이어의 게임 오브젝트가 비활성화된 경우
            // 리스트에서 제거
            pointerIds.RemoveAll(pointer => pointer == null || (pointer.gameObject != null && !pointer.gameObject.activeInHierarchy));

            // 2. 현재 유효한 플레이어의 userId 목록 생성
            HashSet<int> validUserIds = new HashSet<int>();
            foreach (var pointer in pointerIds)
            {
                validUserIds.Add(pointer.userId);
            }

            // 3. 딕셔너리에서 유효하지 않은 userId 제거
            RemoveInvalidEntries(leftHoverStates, validUserIds);
            RemoveInvalidEntries(rightHoverStates, validUserIds);
            RemoveInvalidEntries(isProcessingClick, validUserIds);

            // 4. pointerMapping에서 유효하지 않은 userId에 해당하는 항목 제거
            List<int> keysToRemove = new List<int>();
            foreach (var key in pointerMapping.Keys)
            {
                var mapping = pointerMapping[key];
                if (!validUserIds.Contains(mapping.userId))
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
            {
                pointerMapping.Remove(key);
            }

            // 5. 씬 내의 Player 태그 오브젝트 검색 및 새 플레이어 추가
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerObjects)
            {
                PlayerPointerId pointer = player.GetComponent<PlayerPointerId>();
                if (pointer != null && !pointerIds.Contains(pointer))
                {
                    pointerIds.Add(pointer);

                    if (!leftHoverStates.ContainsKey(pointer.userId))
                        leftHoverStates[pointer.userId] = false;
                    if (!rightHoverStates.ContainsKey(pointer.userId))
                        rightHoverStates[pointer.userId] = false;
                    if (!isProcessingClick.ContainsKey(pointer.userId))
                        isProcessingClick[pointer.userId] = false;

                    // 새 플레이어에 대한 pointerMapping 등록 (최적화)
                    foreach (int id in pointer.leftPointerIds)
                    {
                        if (!pointerMapping.ContainsKey(id))
                            pointerMapping.Add(id, (pointer.userId, true));
                    }
                    foreach (int id in pointer.rightPointerIds)
                    {
                        if (!pointerMapping.ContainsKey(id))
                            pointerMapping.Add(id, (pointer.userId, false));
                    }
                    Debug.Log($"[TextButton] 플레이어 {pointer.userId} 자동 참조 완료");
                }
            }
        }
    }

    // 플레이어가 게임 도중 제거될 때 딕셔너리에서 유효하지 않은 키 제거
    private void RemoveInvalidEntries(Dictionary<int, bool> dict, HashSet<int> validUserIds)
    {
        List<int> keysToRemove = new List<int>();
        foreach (var key in dict.Keys)
        {
            if (!validUserIds.Contains(key))
                keysToRemove.Add(key);
        }
        foreach (var key in keysToRemove)
        {
            dict.Remove(key);
        }
    }

    // InputAction 구독 및 활성화
    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 액션
            string leftActionName = GetActionName(leftSelectActionType);
            leftSelectAction = inputActionAsset.FindAction(leftActionName, true);
            if (leftSelectAction != null)
            {
                leftSelectAction.performed += OnSelectPerformed;
                leftSelectAction.Enable();
                Debug.Log($"[TextButton] '{leftActionName}' 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError($"[TextButton] '{leftActionName}' 액션을 찾을 수 없습니다.");
            }

            // 우측 액션
            string rightActionName = GetActionName(rightSelectActionType);
            rightSelectAction = inputActionAsset.FindAction(rightActionName, true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectPerformed;
                rightSelectAction.Enable();
                Debug.Log($"[TextButton] '{rightActionName}' 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError($"[TextButton] '{rightActionName}' 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다.");
        }
    }

    // InputAction 구독 해제 및 비활성화
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

    // 열거형에 따른 액션 이름 반환
    private string GetActionName(XRActionType actionType)
    {
        switch (actionType)
        {
            case XRActionType.LeftHandInteractionSelect:
                return "XRI LeftHand Interaction/Select";
            case XRActionType.RightHandInteractionSelect:
                return "XRI RightHand Interaction/Select";
            default:
                return "";
        }
    }

    // Select 액션 수행 시 처리
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        bool processed = false;
        foreach (var pointer in pointerIds)
        {
            if (pointer == null)
                continue;

            bool leftHover = leftHoverStates.ContainsKey(pointer.userId) ? leftHoverStates[pointer.userId] : false;
            bool rightHover = rightHoverStates.ContainsKey(pointer.userId) ? rightHoverStates[pointer.userId] : false;

            if ((context.action == leftSelectAction && leftHover) ||
                (context.action == rightSelectAction && rightHover))
            {
                if (isProcessingClick[pointer.userId])
                {
                    Debug.Log($"[TextButton] 플레이어 {pointer.userId}의 입력이 이미 처리 중입니다.");
                    continue;
                }
                Debug.Log($"[TextButton] 플레이어 {pointer.userId}의 입력 처리");
                isProcessingClick[pointer.userId] = true;
                StartCoroutine(TriggerButtonAnimationAndClickForUser(pointer.userId));
                processed = true;
            }
        }
        if (!processed)
            Debug.Log("[TextButton] 해당 컨트롤러의 레이가 버튼 위에 있지 않음");
    }

    // 버튼 클릭 애니메이션 및 이벤트 처리 코루틴
    private IEnumerator TriggerButtonAnimationAndClickForUser(int userId)
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

        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(0.1f);
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        OnButtonClickForUser(userId);
        isProcessingClick[userId] = false;
    }

    // 개별 플레이어 버튼 클릭 처리
    public void OnButtonClickForUser(int userId)
    {
        Debug.Log($"[TextButton] 플레이어 {userId}의 버튼 클릭 처리 완료");
        if (displayText != null)
            displayText.text = $"플레이어 {userId}의 버튼 클릭 처리 완료";
    }

    // 레이 진입 처리
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[TextButton] Pointer Enter: pointerId " + eventData.pointerId);
        if (pointerMapping.TryGetValue(eventData.pointerId, out var mapping))
        {
            if (mapping.isLeft)
            {
                leftHoverStates[mapping.userId] = true;
                Debug.Log($"[TextButton] 플레이어 {mapping.userId}의 좌측 레이 진입");
            }
            else
            {
                rightHoverStates[mapping.userId] = true;
                Debug.Log($"[TextButton] 플레이어 {mapping.userId}의 우측 레이 진입");
            }
        }
    }

    // 레이 종료 처리
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("[TextButton] Pointer Exit: pointerId " + eventData.pointerId);
        if (pointerMapping.TryGetValue(eventData.pointerId, out var mapping))
        {
            if (mapping.isLeft)
            {
                leftHoverStates[mapping.userId] = false;
                Debug.Log($"[TextButton] 플레이어 {mapping.userId} / 좌측 레이 벗어남");
            }
            else
            {
                rightHoverStates[mapping.userId] = false;
                Debug.Log($"[TextButton] 플레이어 {mapping.userId} / 우측 레이 벗어남");
            }
        }
        if (AreAllPointersNotHovering())
        {
            if (resetCoroutine != null)
                StopCoroutine(resetCoroutine);
            resetCoroutine = StartCoroutine(ResetTextAndButtonCoroutine());
        }
    }

    // 모든 플레이어에 대해 레이가가 버튼 위에 있지 않은지 확인
    private bool AreAllPointersNotHovering()
    {
        foreach (var state in leftHoverStates.Values)
        {
            if (state)
                return false;
        }
        foreach (var state in rightHoverStates.Values)
        {
            if (state)
                return false;
        }
        return true;
    }

    // 일정 시간 후 텍스트 복원 코루틴
    private IEnumerator ResetTextAndButtonCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (displayText != null)
            displayText.text = originalText;
    }
}
