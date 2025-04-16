using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using XRDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;
using XRDevices = UnityEngine.XR.InputDevices;
using UnityEditor;

public class EmergencyBellButton : MonoBehaviour
{
    [Header("EmergencyBell 버튼")]
    public Button emergencyBell;

    [Header("highlighter")]
    public Highlighter highlighter;

    [Header("XRI Default Input Actions")]
    public InputActionAsset inputActionAsset;

    // PC 에디터에서 사용하기 위한 오른손 Select 액션
    private InputAction rightSelectAction;

    // 버튼 실행 여부
    private bool isButton;

    // 버튼 클릭 시 다른 스크립트에 알리기 위한 이벤트 -> 알림을 받으면 다음 스텝으로 진행
    public static event Action OnEmergencyBellClicked;

    private void Awake()
    {
        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset -> null");
            return;
        }

        // XRI RightHand Interaction에서 Select 액션을 찾음
        var rightHandInteractionMap = inputActionAsset.FindActionMap("XRI RightHand Interaction", true);
        if (rightHandInteractionMap != null)
        {
            rightSelectAction = rightHandInteractionMap.FindAction("Select", true);
            if (rightSelectAction != null)
            {
                rightSelectAction.performed += OnSelectActionPerformed;
                rightSelectAction.Enable();
                Debug.Log("우측 컨트롤러 Select 액션 활성화");
            }
            else
            {
                Debug.LogError("우측 컨트롤러의 Select 액션을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("XRI RightHand Interaction을 찾을 수 없습니다.");
        }
    }

    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        // PC 에디터에서 우측 컨트롤러의 Select 액션 입력이 되었을 때 호출
        Debug.Log("우측 컨트롤러 Select 액션 입력 감지");
    }

    // 버튼 이벤트 등록
    private void Start()
    {
        if (emergencyBell != null)
            emergencyBell.onClick.AddListener(HandleButtonClick);
        else
            Debug.LogError("EmergencyBell 버튼 -> null");
    }

    // 버튼 노란색 표시 및 깜빡이 켜기
    public void Highlighter()
    {
        // 노란색 표시
        highlighter.SetColor(Color.yellow);

        // 깜빡이 켜기
        highlighter.isBlinking = true;
    }

    // 버튼 이벤트 실행
    private void HandleButtonClick()
    {
        // 다른 스크립트에 실행했음을 전달
        OnEmergencyBellClicked?.Invoke();

        // 버튼 실행 여부 -> 참
        isButton = true;
    }

    // 트리거 영역 내에 있을 때 호출
    private void OnTriggerStay(Collider other)
    {
        // 충돌한 객체의 태그가 Hand2인지 확인
        if (!other.CompareTag("Hand2"))
            return;

        // 버튼 실행
        if (isButton)
        {
            highlighter.SetColor(Color.black);
            highlighter.isBlinking = false;
        }

        // 버튼 실행 x
        if (!isButton)
        {
            // 빨간색 표시
            highlighter.SetColor(Color.red);
        }

        // 충돌 표시
        Debug.Log("Hand2/콜라이더와 충돌 중");

        // 우측 컨트롤러 그립 버튼 입력 여부 
        bool inputDetected = false;

        // 실물 = 우측 컨트롤러의 그립 버튼 입력 확인
        List<XRDevice> devices = new List<XRDevice>();
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        XRDevices.GetDevicesWithCharacteristics(characteristics, devices);

        foreach (XRDevice device in devices)
        {
            bool gripPressed = false;
            if (device.TryGetFeatureValue(XRCommonUsages.gripButton, out gripPressed) && gripPressed)
            {
                //우측 컨트롤러 그립 버튼 입력 시 true 변경
                inputDetected = true;
                Debug.Log("우측 VR 컨트롤러 그립 버튼 입력 감지 - EmergencyBell 실행");
                break;
            }
        }

        // PC = 우측 Select 액션 입력 확인
        if (!inputDetected && rightSelectAction != null && rightSelectAction.triggered)
        {
            //우측 컨트롤러 그립 버튼 입력 시 true 변경
            inputDetected = true;
            Debug.Log("PC 에디터의 우측 Select 액션 입력 감지 - EmergencyBell 실행");
        }

        // 우측 컨트롤러 그립 버튼 입력 시 true 변경 -> HandleButtonClick 실행
        if (inputDetected)
        {
            // 버튼 이벤트 실행
            HandleButtonClick();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 충돌한 객체의 태그가 Hand2인지 확인
        if (!other.CompareTag("Hand2"))
            return;

        // 버튼 실행 x
        if (!isButton)
        {
            // 노란색 표시
            highlighter.SetColor(Color.yellow);
        }
    }
}
