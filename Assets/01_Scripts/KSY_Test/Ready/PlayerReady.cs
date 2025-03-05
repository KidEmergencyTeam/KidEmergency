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

        // 만약 VR 컨트롤러 입력이 유효하지 않다면 (즉, VR 컨트롤러가 없거나 사용 불가능한 상태)
        // 키보드의 스페이스바 입력을 확인하여 준비 처리를 진행
        // 주의: 이 경우 여러 플레이어가 있더라도 한 명이 스페이스바를 누르면 해당 스크립트가 실행 -> 키보드 입력은 개별 처리를 위해 별도의 로직이 필요
        if (!leftDevice.isValid && !rightDevice.isValid)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RegisterReady();
            }
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

    // 플레이어 준비 완료 처리 및 불 함수 실행
    void RegisterReady()
    {
        if (IsReady)
            return;

        IsReady = true;
        Debug.Log(gameObject.name + " 준비 완료!");

        // 여기서 불 함수를 실행할 수 있습니다.
        // 예를 들어, 불 효과 애니메이션을 재생하거나 불 이펙트를 활성화하는 코드를 넣을 수 있음

        // 준비 완료 이벤트 발생
        onPlayerReady?.Invoke();
    }
}
