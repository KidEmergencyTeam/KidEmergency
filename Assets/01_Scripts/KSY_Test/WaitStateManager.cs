using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement; 

public class WaitStateManager : MonoBehaviour
{
    [Header("스폰 위치")]
    public Transform spawnPoint;      
    
    [Header("플레이어")]
    public GameObject player;          
    
    [Header("준비 상태를 표시")]
    public TMP_Text statusUIText;      
    
    [Header("게임 씬")]
    public string sceneName;           

    private bool isReady = false;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Start()
    {
        // 초기 스폰 위치로 플레이어 이동
        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
        UpdateUI("대기중...");

        // 왼손 컨트롤러 찾기
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftCharacteristics, devices);
        if (devices.Count > 0)
            leftDevice = devices[0];

        // 오른손 컨트롤러 찾기
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
            // pc -> 키보드 입력 G만 체크
            if (!leftDevice.isValid && !rightDevice.isValid)
            {
                if (Input.GetKey(KeyCode.G))
                {
                    isReady = true;
                    UpdateUI("준비 완료");
                    Invoke("StartGame", 2f);
                }
            }

            // VR -> 각 컨트롤러의 그립 버튼 입력을 체크
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
                    isReady = true;
                    UpdateUI("준비 완료");
                    Invoke("StartGame", 2f);
                }
            }
        }
    }

    void UpdateUI(string status)
    {
        if (statusUIText != null)
            statusUIText.text = status;
    }

    void StartGame()
    {
        Debug.Log("게임 시작!");
        // 할당된 씬 이름으로 씬 전환
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("sceneName이 할당되지 않았습니다.");
        }
    }
}
