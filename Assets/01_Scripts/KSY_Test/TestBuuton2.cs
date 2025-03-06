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

    public void OnButtonClick()
    {
        Debug.Log("[TestButton2] OnButtonClick() 호출됨.");
    }
}
