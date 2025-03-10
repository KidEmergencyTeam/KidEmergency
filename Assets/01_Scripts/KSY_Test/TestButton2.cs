using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // Select를 기본값으로 사용
    [Header("XRI Default Input Actions 내에서 Select(Grip) 액션의 이름")]
    public string selectActionName = "Select";

    // Select 액션을 저장
    private InputAction selectAction;

    // 레이가 버튼 위에 있는지 여부
    private bool isRayHovering = false;

    // OnButtonClick() 호출 여부를 인스펙터에 표시
    [Header("디버그 (호출 여부)")]
    [SerializeField]
    private bool buttonClicked = false;

    void Start()
    {
        if (button == null)
        {
            Debug.LogError("[TestButton2] Button이 할당되지 않았습니다.");
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
            }
            else
            {
                Debug.LogError("[TestButton2] '" + selectActionName + "' 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestButton2] InputActionAsset이 할당되지 않았습니다.");
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

    // 레이가 버튼 위에 있을 때 XR 컨트롤러의 Grip 입력 발생 시
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (isRayHovering)
        {
            // XR 입력 시 버튼 애니메이션 효과를 시뮬레이션하고 클릭 이벤트 호출
            StartCoroutine(TriggerButtonAnimationAndClick());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isRayHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isRayHovering = false;
    }

    // XR 입력 시 버튼 애니메이션 및 클릭 처리
    private IEnumerator TriggerButtonAnimationAndClick()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("[TestButton2] EventSystem.current가 null입니다.");
            yield break;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.pointerPress = button.gameObject;

        // pointer down 이벤트를 시뮬레이션하여 눌림 효과 시작
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);

        // 애니메이션 효과를 보기 위해 잠시 대기 (0.1초 정도)
        yield return new WaitForSeconds(0.1f);

        // pointer up 이벤트를 시뮬레이션하여 눌림 효과 종료
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);

        // 버튼 클릭 이벤트 호출
        OnButtonClick();
    }

    // XR 입력 시 호출
    public void OnButtonClick()
    {
        buttonClicked = true;
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
