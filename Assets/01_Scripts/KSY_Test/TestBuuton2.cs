using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestBuuton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public InputActionAsset inputActionAsset;
    public string selectActionName = "Select";

    private InputAction selectAction;
    private bool isRayHovering = false;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("[TestBuuton2] Button이 할당되지 않았습니다.");
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
                Debug.LogError("[TestBuuton2] '" + selectActionName + "' 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TestBuuton2] InputActionAsset이 할당되지 않았습니다.");
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
        Debug.Log("[TestBuuton2] OnButtonClick() 호출됨.");
    }
}
