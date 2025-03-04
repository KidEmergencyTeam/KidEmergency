using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// 대기 상태에서 준비 완료되면 다음 씬으로 전환 시켜주는 스크립트
public class WaitState : MonoBehaviour
{
    [Header("대기 상태 표시")]
    public TextMeshProUGUI WaitStateText;

    [Header("다음 씬 이름")]
    public string nextSceneName;

    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    // 로컬 플레이어 준비 여부
    private bool isReady = false;

    // 컨트롤러
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Start()
    {
        UpdateUI("대기중: 0/1");

        // 왼쪽 컨트롤러 입력 확인
        var devices = new List<InputDevice>();
        InputDeviceCharacteristics leftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftCharacteristics, devices);
        if (devices.Count > 0)
            leftDevice = devices[0];

        // 오른쪽 컨트롤러 입력 확인
        devices.Clear();
        InputDeviceCharacteristics rightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightCharacteristics, devices);
        if (devices.Count > 0)
            rightDevice = devices[0];
    }

    void Update()
    {
        if (!isReady)
        {
            // 컨트롤러가 유효하지 않으면 키보드 Space키로 준비 처리
            if (!leftDevice.isValid && !rightDevice.isValid)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RegisterReady();
                }
            }

            // 컨트롤러가 존재하면 그랩 버튼으로 준비 처리
            else
            {
                bool leftGripPressed = false;
                bool rightGripPressed = false;

                if (leftDevice.isValid)
                    leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out leftGripPressed);
                if (rightDevice.isValid)
                    rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out rightGripPressed);

                if (leftGripPressed && rightGripPressed)
                {
                    RegisterReady();
                }
            }
        }
    }

    // 플레이어 준비 완료 후 씬 전환 실행
    void RegisterReady()
    {
        if (isReady)
            return;

        isReady = true;

        UpdateUI("준비 완료: 1/1");
        Debug.Log("플레이어 준비 완료! 게임 시작 대기...");

        Invoke("StartGame", gameStartDelay);
    }

    // UI 업데이트 처리
    void UpdateUI(string status)
    {
        if (WaitStateText != null)
            WaitStateText.text = status;
    }

    // 씬 전환 실행
    void StartGame()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("다음 씬 이름이 설정되어 있지 않습니다.");
            return;
        }

        Debug.Log("게임 시작!");
        SceneManager.LoadScene(nextSceneName);
    }
}
