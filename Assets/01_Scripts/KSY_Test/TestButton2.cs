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

    // OnButtonClick() 호출 여부를 인스펙터에 표시하기 위한 불변수
    [Header("디버그 (호출 여부)")]
    [SerializeField]
    private bool buttonClicked = false;

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
        // XRI Default Input Actions가 할당되어 있는지 확인
        if (inputActionAsset != null)
        {
            // XRI Default Input Actions에서
            // selectActionName에 해당하는 액션을 찾음.
            selectAction = inputActionAsset.FindAction(selectActionName, true);

            if (selectAction != null)
            {
                // selectAction 수행 시 OnSelectPerformed 호출
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

    // 레이가 버튼 위에 있을 때 XR 컨트롤러의 Grip 입력이 발생하면 버튼 클릭 처리
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (isRayHovering)
        {
            button.onClick.Invoke();
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

    // 버튼 클릭 이벤트
    public void OnButtonClick()
    {
        // OnButtonClick()이 호출되면 buttonClicked 변수를 true로 변경
        buttonClicked = true;
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
