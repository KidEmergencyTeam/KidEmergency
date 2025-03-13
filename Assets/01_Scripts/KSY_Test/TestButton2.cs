using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

// Select 액션 타입 열거형 정의
public enum XRActionType
{
    LeftHandInteractionSelect,
    RightHandInteractionSelect
}

// 개별적으로 플레이어 및 입력을 구분하여 버튼 클릭 시 이벤트를 호출하는 스크립트  
// 따라서 버튼 이벤트는 따로 스크립트를 만들어서 처리하면 됨
public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("Select 액션 타입")]
    public XRActionTypeTest leftSelectActionType = XRActionTypeTest.LeftHandInteractionSelect;
    public XRActionTypeTest rightSelectActionType = XRActionTypeTest.RightHandInteractionSelect;

    [Header("포인터 ID 관리 스크립트")]
    public List<PlayerPointerId> pointerIds;

    [Header("디버그 (호출 여부)")]
    public bool buttonClicked = false;

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

    // 플레이어 검색 주기 (초)
    private float checkInterval = 1f;
    private float lastCheckTime = 0f;

    void Start()
    {
        // pointerIds가 null 상태인 경우를 명시적으로 피하기 위해 새로운 빈 리스트를 생성
        if (pointerIds == null)
            pointerIds = new List<PlayerPointerId>();
    }

    void Update()
    {
        // 일정 시간 간격으로 플레이어 목록 갱신
        if (Time.time - lastCheckTime > checkInterval)
        {
            lastCheckTime = Time.time;

            // 1. pointerIds 리스트에 할당된 pointer.cs가 null 상태이거나
            // 해당 플레이어의 게임 오브젝트가 비활성화된 경우 리스트에서 제거
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

                    // 새 플레이어의 포인터 ID 등록 (좌측/우측 구분)
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
                    Debug.Log($"[TestButton2] 플레이어 {pointer.userId} 자동 참조 완료");
                }
            }
        }
    }

    // 딕셔너리에서 유효하지 않은 userId 제거
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
                Debug.Log($"[TestButton2] '{leftActionName}' 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError($"[TestButton2] '{leftActionName}' 액션을 찾을 수 없습니다.");
            }

            // 우측 액션
            string rightActionName = GetActionName(rightSelectActionType);
            rightSelectAction = inputActionAsset.FindAction(rightActionName, true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectPerformed;
                rightSelectAction.Enable();
                Debug.Log($"[TestButton2] '{rightActionName}' 액션 구독 및 활성화");
            }
            else
            {
                Debug.LogError($"[TestButton2] '{rightActionName}' 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestButton2] InputActionAsset이 할당되지 않았습니다.");
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
    private string GetActionName(XRActionTypeTest actionType)
    {
        switch (actionType)
        {
            case XRActionTypeTest.LeftHandInteractionSelect:
                return "XRI LeftHand Interaction/Select";
            case XRActionTypeTest.RightHandInteractionSelect:
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

            // 좌측 혹은 우측 액션에 대해 레이가 버튼 위에 있을 경우 처리
            if ((context.action == leftSelectAction && leftHover) ||
                (context.action == rightSelectAction && rightHover))
            {
                if (isProcessingClick[pointer.userId])
                {
                    Debug.Log($"[TestButton2] 플레이어 {pointer.userId}의 입력이 이미 처리 중입니다.");
                    continue;
                }
                Debug.Log($"[TestButton2] 플레이어 {pointer.userId}의 입력 처리");
                isProcessingClick[pointer.userId] = true;
                StartCoroutine(TriggerButtonAnimationAndClickForUser(pointer.userId));
                processed = true;
            }
        }
        if (!processed)
            Debug.Log("[TestButton2] 해당 컨트롤러의 레이가 버튼 위에 있지 않음");
    }

    // 버튼 클릭 애니메이션 및 이벤트 처리 코루틴
    private IEnumerator TriggerButtonAnimationAndClickForUser(int userId)
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

        OnButtonClickForUser(userId);
        isProcessingClick[userId] = false;
    }

    // 개별 플레이어 버튼 클릭 처리
    public void OnButtonClickForUser(int userId)
    {
        Debug.Log($"[TestButton2] 플레이어 {userId}의 버튼 클릭 처리 완료");
        // 여기서 버튼 클릭 후 텍스트 출력 또는 다른 처리 로직 추가 가능
        buttonClicked = true;
    }

    // 레이 진입 처리
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[TestButton2] Pointer Enter: pointerId " + eventData.pointerId);
        if (pointerMapping.TryGetValue(eventData.pointerId, out var mapping))
        {
            if (mapping.isLeft)
            {
                leftHoverStates[mapping.userId] = true;
                Debug.Log($"[TestButton2] 플레이어 {mapping.userId}의 좌측 레이 진입");
            }
            else
            {
                rightHoverStates[mapping.userId] = true;
                Debug.Log($"[TestButton2] 플레이어 {mapping.userId}의 우측 레이 진입");
            }
        }
    }

    // 레이 종료 처리
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("[TestButton2] Pointer Exit: pointerId " + eventData.pointerId);
        if (pointerMapping.TryGetValue(eventData.pointerId, out var mapping))
        {
            if (mapping.isLeft)
            {
                leftHoverStates[mapping.userId] = false;
                Debug.Log($"[TestButton2] 플레이어 {mapping.userId} / 좌측 레이 벗어남");
            }
            else
            {
                rightHoverStates[mapping.userId] = false;
                Debug.Log($"[TestButton2] 플레이어 {mapping.userId} / 우측 레이 벗어남");
            }
        }
    }
}
