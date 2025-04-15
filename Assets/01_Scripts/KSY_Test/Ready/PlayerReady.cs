using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerReady : MonoBehaviour
{
    // 플레이어 준비 상태
    public bool IsReady { get; private set; } = false;

    // 준비 완료 이벤트 (다른 스크립트에서 등록하여 콜백 받을 수 있음)
    public delegate void PlayerReadyDelegate();
    public event PlayerReadyDelegate onPlayerReady;

    // XR 입력 장치
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Start()
    {
        // 왼쪽 컨트롤러 초기화
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftChar = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftChar, devices);
        if (devices.Count > 0)
            leftDevice = devices[0];

        // 오른쪽 컨트롤러 초기화
        devices.Clear();
        InputDeviceCharacteristics rightChar = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightChar, devices);
        if (devices.Count > 0)
            rightDevice = devices[0];
    }

    void Update()
    {
        // 만약 플레이어가 이미 준비된 상태라면
        // 더 이상의 입력 처리를 하지 않고 종료
        if (IsReady)
            return;

        // 컨트롤러가 사용 불가능한 상태 -> 준비 완료 처리
        if (!leftDevice.isValid && !rightDevice.isValid)
        {
            RegisterReady();
        }
        else
        {
            // VR 컨트롤러가 유효할 경우, 각각의 컨트롤러의 그립 버튼 입력을 체크
            bool leftGrip = false;
            bool rightGrip = false;
            if (leftDevice.isValid)
                leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
            if (rightDevice.isValid)
                rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);

            // 만약 양쪽 컨트롤러의 그립 버튼이 모두 눌려있다면 준비 상태로 전환
            if (leftGrip && rightGrip)
            {
                RegisterReady();
            }
        }
    }

    // 플레이어 준비 완료 처리
    void RegisterReady()
    {
        if (IsReady)
            return;

        IsReady = true;
        Debug.Log(gameObject.name + " 준비 완료!");

        // 준비 완료 
        onPlayerReady?.Invoke();
    }
}
