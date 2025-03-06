using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // IPointerEnterHandler, IPointerExitHandler 사용
using TMPro;
using System.Collections;
using UnityEngine.InputSystem; // 새로운 XR Input System 사용

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public TextMeshProUGUI displayText;  
    public Button button;                

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // InputActionAsset -> Select 불러오기, 기본값임
    [Header("XRI Default Input Actions 내에서 Select(Grip) 액션의 이름")]
    public string selectActionName = "Select";   
    
    private InputAction selectAction;             

    // 원래 텍스트 및 코루틴 관리 변수
    private string originalText;
    private Coroutine resetCoroutine;

    // XR Ray Interactor가 버튼 위에 있는지 여부 (디버그용)
    private bool isRayHovering = false;

    void Start()
    {
        // Button 할당 확인 및 OnClick 이벤트 등록
        if (button != null)
        {
            Debug.Log("[TextButton] Button 할당됨: " + button.name);
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("[TextButton] Button이 할당되지 않았습니다. Inspector에서 확인하세요.");
        }

        // displayText 할당 확인 및 원래 텍스트 저장
        if (displayText != null)
        {
            Debug.Log("[TextButton] displayText 할당됨: " + displayText.name);
            originalText = displayText.text;
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 할당되지 않았습니다. Inspector에서 확인하세요.");
        }
    }

    void OnEnable()
    {
        if (inputActionAsset != null)
        {
            // inputActionAsset에서 selectActionName에 해당하는 액션을 찾음 -> 그립 버튼 처리를 위한 개념
            selectAction = inputActionAsset.FindAction(selectActionName, true);
            if (selectAction != null)
            {
                selectAction.performed += OnSelectPerformed;
                selectAction.Enable();
                Debug.Log("[TextButton] '" + selectActionName + "' 액션 구독 및 활성화됨.");
            }
            else
            {
                Debug.LogError("[TextButton] '" + selectActionName + "' 액션을 InputActionAsset에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("[TextButton] InputActionAsset이 할당되지 않았습니다. Inspector에서 확인하세요.");
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

    // XR Input System의 Select(Grip) 액션이 수행되었을 때 호출
    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[TextButton] Select(Grip) 액션 performed. isRayHovering: " + isRayHovering);
        if (isRayHovering)
        {
            Debug.Log("[TextButton] Grip 버튼 입력 및 Ray Hovering 상태, 버튼 클릭 처리");
            // UI Button의 OnClick 이벤트 호출
            button.onClick.Invoke();
        }
    }

    // XR Ray Interactor가 버튼 위로 진입 시 호출 (IPointerEnterHandler)
    public void OnPointerEnter(PointerEventData eventData)
    {
        isRayHovering = true;
        Debug.Log("[TextButton] Ray가 버튼 위에 진입함: " + eventData.pointerEnter);
    }

    // XR Ray Interactor가 버튼 영역에서 벗어날 때 호출 (IPointerExitHandler)
    public void OnPointerExit(PointerEventData eventData)
    {
        isRayHovering = false;
        Debug.Log("[TextButton] Ray가 버튼 영역에서 벗어남: " + eventData.pointerEnter);

        // 버튼 영역에서 벗어나면 일정 시간 후 원래 텍스트 복원
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(ResetTextCoroutine());
    }

    // UI Button의 OnClick 이벤트에 의해 호출
    public void OnButtonClick()
    {
        Debug.Log("[TextButton] OnButtonClick() 호출됨 (UI Button 클릭 이벤트).");
        if (displayText != null)
        {
            displayText.text = "버튼 클릭 처리 완료!";
            Debug.Log("[TextButton] displayText 업데이트: " + displayText.text);

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetTextCoroutine());
        }
        else
        {
            Debug.LogError("[TextButton] displayText가 null입니다. 텍스트 변경 불가.");
        }
    }

    // 2초 후 원래 텍스트로 복원하는 코루틴
    IEnumerator ResetTextCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (displayText != null)
        {
            displayText.text = originalText;
            Debug.Log("[TextButton] displayText 원래 텍스트로 복원됨: " + originalText);
        }
        resetCoroutine = null;
    }
}
