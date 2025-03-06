using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼")]
    public Button button;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // Select을 기본값으로 사용
    [Header("XRI Default Input Actions 내에서 Select(Grip) 액션의 이름")]
    public string selectActionName = "Select";

    // Select 액션을 저장하는 용도 -> 효율성을 위한 개념
    private InputAction selectAction;

    // 레이가 버튼 위에 있는지 여부
    private bool isRayHovering = false;

    void Start()
    {
        // 버튼에 이벤트 등록
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("[TestButton2] Button이 할당되지 않았습니다.");
        }
    }

    void OnEnable()
    {
        // XRI Default Input Actions 할당되어 있는지 확인
        if (inputActionAsset != null)
        {
            // XRI Default Input Actions에서
            // selectActionName에 해당하는 액션을 찾음.
            // 해당 액션이 없으면 LogError 출력
            selectAction = inputActionAsset.FindAction(selectActionName, true);

            // selectActionName에 해당하는 액션을 찾았을 경우
            if (selectAction != null)
            {
                // selectAction이 수행되었을 때
                // OnSelectPerformed 함수를 호출하도록 이벤트 구독
                selectAction.performed += OnSelectPerformed;

                // selectAction을 활성화
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
        // selectAction이 null이 아닌 경우에만
        // 이벤트 구독 해제 및 비활성화 처리
        if (selectAction != null)
        {
            // OnSelectPerformed 이벤트 구독 해제
            selectAction.performed -= OnSelectPerformed;
            // selectAction을 비활성화
            selectAction.Disable();
        }
    }

    // 레이가 버튼 위에 있는 경우
    // XR 컨트롤러 그립 입력이 발생하면
    // 1.button.onClick.Invoke()를 호출
    // 2.버튼 클릭 이벤트를 실행
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (isRayHovering)
        {
            button.onClick.Invoke();
        }
    }

    // 레이가 버튼 위에 있을 경우
    public void OnPointerEnter(PointerEventData eventData)
    {
        isRayHovering = true;
    }

    // 레이가 버튼 위에 없을 경우
    public void OnPointerExit(PointerEventData eventData)
    {
        isRayHovering = false;
    }

    // 버튼 클릭 이벤트 
    public void OnButtonClick()
    {
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
