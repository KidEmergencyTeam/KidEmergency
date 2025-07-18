using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public enum ButtonType
{
    A,
    B
}

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Button button;

    [Header("버튼 식별")]
    public ButtonType buttonType;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // RayController2에 대한 참조 -> 태그로 찾음
    private RayController2 rayController;

    // 좌측, 우측 컨트롤러의 Select 액션
    private InputAction leftSelectAction;
    private InputAction rightSelectAction;

    // Pointer 이벤트로 레이의 진입 상태를 관리
    private bool isHovered = false;

    // 버튼 클릭 상태를 관리 -> JDH 전용
    public bool isClick = false;

    // 현재 입력된 그립 상태를 RayType으로 변환하는 과정에서 문제가 발생했을 때 버튼 실행을 막기 위한 플래그
    private bool isValidGrip;  

    // 딕셔너리를 통한 버튼 타입에 따른
    // 버튼 클릭 이벤트 처리
    private Dictionary<ButtonType, System.Action> buttonClickActions;

    // 상수 선언
    private const string LEFT_SELECT_ACTION = "XRI LeftHand Interaction/Select";
    private const string RIGHT_SELECT_ACTION = "XRI RightHand Interaction/Select";
    private const string LEFT_STRING = "Left";
    private const string RIGHT_STRING = "Right";

    private void Awake()
    {
        // 버튼 클릭 이벤트 할당
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("[TestButton2] Button 컴포넌트가 할당되지 않았습니다.");
        }

        // 딕셔너리 등록 -> 버튼 타입에 따른 액션 할당
        buttonClickActions = new Dictionary<ButtonType, System.Action>
        {
            { ButtonType.A, () => Debug.Log("버튼 A 클릭 이벤트 발생") },
            { ButtonType.B, () => Debug.Log("버튼 B 클릭 이벤트 발생") }
        };
    }

    private void Start()
    {
        // "Player" 태그를 사용하여 RayController2.cs 할당
        GameObject rayControllerObj = GameObject.FindGameObjectWithTag("Player");
        if (rayControllerObj != null)
        {
            rayController = rayControllerObj.GetComponent<RayController2>();
        }
        else
        {
            Debug.LogWarning("[TestButton2] 'RayController' 태그가 지정된 오브젝트를 찾지 못했습니다.");
        }
    }

    // Select 액션 등록
    private void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // 좌측 컨트롤러 Select 액션 구독 및 활성화
            leftSelectAction = inputActionAsset.FindAction(LEFT_SELECT_ACTION, true);
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
            rightSelectAction = inputActionAsset.FindAction(RIGHT_SELECT_ACTION, true);
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

    // Select 액션 해제
    private void OnDisable()
    {
        // 좌측 해제
        if (leftSelectAction != null)
        {
            leftSelectAction.performed -= OnSelectActionPerformed;
            leftSelectAction.Disable();
        }

        // 우측 해제
        if (rightSelectAction != null)
        {
            rightSelectAction.performed -= OnSelectActionPerformed;
            rightSelectAction.Disable();
        }
    }

    // 해당 객체 제거될 때,
    // 버튼 클릭 이벤트 제거
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    // 스크립트에서 IPointerEnterHandler를 추가했을 경우,
    // 레이가 버튼 위에 있을 때 OnPointerEnter 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 버튼 위에 레이가 있다.
        isHovered = true;

        // RayController2.cs -> ActiveRay를 사용하여 버튼 위에 있는 레이를 판단
        Debug.Log($"[TestButton2] {buttonType} 버튼 위에 {rayController.ActiveRay} 레이 들어옴");
    }

    // 스크립트에서 IPointerExitHandler를 추가했을 경우,
    // 레이가 버튼 위에 없을 때 OnPointerExit 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 버튼 위에 레이가 없다.
        isHovered = false;

        // RayController2.cs -> ActiveRay를 사용하여 버튼 위에 있는 레이를 판단
        Debug.Log($"[TestButton2] {buttonType} 버튼 위에 {rayController.ActiveRay} 레이 나감");
    }

    // 조건을 만족하면 버튼 실행
    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        // 디버그 출력을 위한 문자열 변수 선언
        string leftOrRight = "";

        // context.action을 사용하여 어느 쪽에서 호출되었는지 구분하고,
        // 해당 값을 문자열 변수에 대입한 후, 그에 따라 디버그 메시지를 출력
        if (context.action == leftSelectAction)
        {
            leftOrRight = LEFT_STRING;
        }
        else if (context.action == rightSelectAction)
        {
            leftOrRight = RIGHT_STRING;
        }

        // 클래스 밖에서 선언된 개념은 별도로 특정 스크립트를 찾지 않아도 참조 가능

        // 버튼 실행 조건문에서
        // 현재 활성화된 레이의 RayType 열거형 변수와
        // 현재 입력된 그립 상태를 비교하기 위해
        // 현재 입력된 그립 상태를 RayType 열거형 변수로 저장
        RayType gripType;

        // leftOrRight 문자열("왼쪽", "오른쪽")을 RayType으로 변환
        if (leftOrRight == LEFT_STRING)
        {
            gripType = RayType.Left;
            isValidGrip = true;
        }
        else if (leftOrRight == RIGHT_STRING)
        {
            gripType = RayType.Right;
            isValidGrip = true;
        }
        else
        {
            Debug.LogError("현재 입력된 그립 상태를 RayType으로 변환 실패");
            isValidGrip = false;  
            return;
        }

        // 버튼 위에 레이가 있고, 현재 활성화된 레이와 그립 상태가 일치할 때만 실행
        if (isHovered && rayController.ActiveRay == gripType)
        {
            StartCoroutine(TriggerButtonAnimationAndClick());
            Debug.Log($"[TestButton2] 입력된 {leftOrRight} 그립과 활성화된 {rayController.ActiveRay} 레이가 일치하여 버튼 실행");
        }
        else
        {
            Debug.Log("[TestButton2] 버튼 실행 조건 불충족");
            isValidGrip = false;
        }
    }

    // 버튼 눌림 효과 및 버튼 클릭 이벤트 실행
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        // EventSystem.current를 통해 마우스/터치 이벤트를 전달받으므로,
        // EventSystem.current가 null일 경우에는 이벤트 처리가 불가능하여 코루틴을 종료
        if (EventSystem.current == null)
        {
            Debug.LogError("[TestButton2] EventSystem.current가 null입니다.");
            yield break;
        }

        // 현재 활성화된 EventSystem.current을 사용하여 새로운 PointerEventData 객체를 생성 -> 눌림 효과에서만 사용
        // 이 객체는 입력 이벤트(예: 포인터 클릭, 드래그 등)에 대한 정보를 담는다.
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            // PointerEventData 스크립트 pointerPress 속성에 해당 버튼을 할당
            // 이를 통해 어느 버튼을 눌렀는지 특정 가능
            pointerPress = button.gameObject
        };

        // 그립 버튼을 누르면 버튼 눌림 효과 시작
        // 그립 버튼을 누르고 있는 동안에는 버튼 눌림 상태가 유지 -> 그립 버튼을 떼면 pointerUpHandler 호출 -> 눌림 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 그립 버튼을 떼면 버튼 눌림 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 클릭 이벤트 실행
        OnButtonClicked();

        // JDH 전용
        isClick = true;
    }

    // 버튼 클릭 이벤트 
    private void OnButtonClicked()
    {
        // 그립 상태가 유효하지 않으면, 버튼 실행 중단
        if (!isValidGrip)
        {
            Debug.Log("[TestButton2] 현재 입력된 그립 상태를 RayType으로 변환 실패 따라서 버튼 클릭 이벤트 중단");
            return;
        }

        if (buttonClickActions != null && buttonClickActions.TryGetValue(buttonType, out System.Action action))
        {
            // 버튼 실행 -> 버튼에서 사용되는 Invoke와 일반적으로 사용되는 Invoke 개념은 다르다.
            action.Invoke();
        }
        else
        {
            Debug.Log("미지정 버튼 클릭 이벤트 발생");
        }
    }
}