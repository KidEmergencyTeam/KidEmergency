using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("좌측 Select: XRI LeftHand Interaction/Select")]
    public string leftSelectActionName = "XRI LeftHand Interaction/Select";

    [Header("우측 Select: XRI RightHand Interaction/Select")]
    public string rightSelectActionName = "XRI RightHand Interaction/Select";

    [Header("디버그 (호출 여부)")]
    public bool buttonClicked = false;

    [Header("포인터 ID 관리 스크립트")]
    public List<PlayerPointerId> pointerIds;

    // 좌측 및 우측 Select 액션 저장
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // 좌측, 우측 레이가 버튼 위에 있는지 여부
    private bool isLeftRayHovering = false;
    private bool isRightRayHovering = false;

    void Start()
    {
        // 씬에서 태그가 "Player"인 오브젝트를 찾아 pointerIds 리스트에 할당
        if (pointerIds == null || pointerIds.Count == 0)
        {
            pointerIds = new List<PlayerPointerId>();
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerObjects)
            {
                PlayerPointerId pointer = player.GetComponent<PlayerPointerId>();
                if (pointer != null)
                {
                    pointerIds.Add(pointer);
                }
            }
            Debug.Log("[TextButton] Player 태그의 오브젝트에서 PlayerPointerId 컴포넌트를 자동으로 할당하였습니다.");
        }

        if (button == null)
        {
            Debug.LogError("[TestButton2] Button이 할당되지 않았습니다.");
        }

        if (pointerIds == null || pointerIds.Count == 0)
        {
            Debug.LogError("[TestButton2] PlayerPointerId 리스트가 할당되지 않았거나 비어있습니다.");
        }
    }

    // 이벤트 구독
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
                Debug.Log("[TestButton2] '" + leftSelectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TestButton2] '" + leftSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }

            // 우측 액션 구독 및 활성화
            rightSelectAction = inputActionAsset.FindAction(rightSelectActionName, true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectPerformed;
                rightSelectAction.Enable();
                Debug.Log("[TestButton2] '" + rightSelectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TestButton2] '" + rightSelectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestButton2] InputActionAsset이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

    // 이벤트 구독 해제
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

    // 버튼 위에 레이가 있을 때, 해당 컨트롤러의 입력이 발생하면 처리
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        // 좌측 레이가 버튼 위에 있고 좌측 입력이 발생하거나,
        // 우측 레이가 버튼 위에 있고 우측 입력이 발생할 경우
        if ((context.action == leftSelectAction && isLeftRayHovering) ||
            (context.action == rightSelectAction && isRightRayHovering))
        {
            // XR 입력 시 버튼 애니메이션 효과를 적용하고 버튼 이벤트 호출
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
        else
        {
            Debug.Log("[TestButton2] 해당 컨트롤러의 레이가 버튼 위에 있지 않음.");
        }
    }

    // 버튼 위에 레이가 진입하면, 리스트에 등록된 각 PlayerPointerId를 확인하여 좌측/우측 구분
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 단순하게 버튼 위에 레이가 진입하면, Id 값 디버그 출력 -> 세세하게 x
        Debug.Log("[TestButton2] Pointer Enter: pointerId " + eventData.pointerId);

        // pointerIds 리스트가 null이 아니고 하나 이상의 요소를 포함하고 있을 경우 실행
        if (pointerIds != null && pointerIds.Count > 0)
        {
            // pointerIds 리스트에 있는 각 PlayerPointerId 객체를 순회하며 처리
            foreach (var pointer in pointerIds)
            {
                // 좌측 포인터 ID 배열에 포함되어 있는지 확인
                foreach (int id in pointer.leftPointerIds)
                {
                    if (eventData.pointerId == id)
                    {
                        isLeftRayHovering = true;
                        Debug.Log("[TestButton2] 좌측 레이 진입");
                        break;
                    }
                }

                // 우측 포인터 ID 배열에 포함되어 있는지 확인
                foreach (int id in pointer.rightPointerIds)
                {
                    if (eventData.pointerId == id)
                    {
                        isRightRayHovering = true;
                        Debug.Log("[TestButton2] 우측 레이 진입");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("[TestButton2] PlayerPointerId 리스트가 할당되지 않았거나 비어있음");
        }
    }

    // 버튼 영역을 벗어나면 해당 컨트롤러의 hovering 상태 업데이트
    public void OnPointerExit(PointerEventData eventData)
    {
        // 단순하게 버튼 위에 레이가 영역을 벗어나면, Id 값 디버그 출력 -> 세세하게 x
        Debug.Log("[TestButton2] Pointer Exit: pointerId " + eventData.pointerId);

        // pointerIds 리스트가 null이 아니고 하나 이상의 요소를 포함하고 있을 경우 실행
        if (pointerIds != null && pointerIds.Count > 0)
        {
            // pointerIds 리스트에 있는 각 PlayerPointerId 객체를 순회하며 처리
            foreach (var pointer in pointerIds)
            {
                // 좌측 포인터 ID 배열에 포함되어 있는지 확인
                foreach (int id in pointer.leftPointerIds)
                {
                    if (eventData.pointerId == id)
                    {
                        isLeftRayHovering = false;
                        Debug.Log("[TestButton2] 좌측 레이 벗어남");
                        break;
                    }
                }

                // 우측 포인터 ID 배열에 포함되어 있는지 확인
                foreach (int id in pointer.rightPointerIds)
                {
                    if (eventData.pointerId == id)
                    {
                        isRightRayHovering = false;
                        Debug.Log("[TestButton2] 우측 레이 벗어남");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("[TestButton2] PlayerPointerId 리스트가 할당되지 않았거나 비어있음");
        }
    }

    // XR 입력 시 버튼 애니메이션 및 클릭 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        // EventSystem이 존재하는지 확인
        if (EventSystem.current == null)
        {
            Debug.LogError("[TestButton2] EventSystem.current가 null입니다.");
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
        // 버튼 클릭 시, ChoiceVoteManager.cs(선택지 선택 관련 스크립트) 내에서 선택지 처리를 진행함
        // 따라서 TestButton2.cs에서는 버튼 클릭 시 이벤트만 불러오면 됨.
        buttonClicked = true;
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
