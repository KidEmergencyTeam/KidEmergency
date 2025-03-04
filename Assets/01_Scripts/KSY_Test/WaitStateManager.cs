using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WaitStateManager : MonoBehaviour
{
    [Header("플레이어")]
    public GameObject player;
    [TagSelector]
    public string playerTag;

    [Header("대기 상태 표시")]
    public TextMeshProUGUI WaitStateText;
    [TagSelector]
    public string WaitStateTextTag;

    // 기존 씬 스폰 위치 관련 변수는 필요에 따라 제거 또는 유지하세요.
    [Header("현재 씬 스폰 위치")]
    public Vector3 currentSceneSpawnPosition;

    [Header("다음 씬 이름")]
    public string nextSceneName;

    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    // 로컬 플레이어 준비 여부
    private bool isReady = false;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Awake()
    {
        // playerTag로 플레이어 객체 검색
        if (player == null && !string.IsNullOrEmpty(playerTag))
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
        }
    }

    void Start()
    {
        StartCoroutine(DelayedInitialization());

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame();

        // 현재 씬의 스폰 위치로 플레이어 이동
        if (player != null)
        {
            player.transform.position = currentSceneSpawnPosition;
        }
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
        if (WaitStateText == null)
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(WaitStateTextTag);
            if (tmpObj != null)
            {
                WaitStateText = tmpObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("태그 '" + WaitStateTextTag + "'를 가진 TextMeshProUGUI 오브젝트를 찾을 수 없습니다.");
            }
        }

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

    // 씬 전환 후 UI와 플레이어 위치 업데이트
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject tmpObj = GameObject.FindGameObjectWithTag(WaitStateTextTag);
        if (tmpObj != null)
        {
            WaitStateText = tmpObj.GetComponent<TextMeshProUGUI>();
            Debug.Log("현재 씬에서 TextMeshProUGUI 할당됨.");
        }
        else
        {
            Debug.LogWarning("현재 씬에서 태그 '" + WaitStateTextTag + "'를 가진 TextMeshProUGUI 오브젝트를 찾을 수 없습니다.");
        }

        // playerTag로 플레이어 객체 검색 (없을 경우)
        if (player == null && !string.IsNullOrEmpty(playerTag))
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
        }

        // 필요에 따라 플레이어 위치 업데이트 (예: 기본 위치 또는 다른 로직 적용)
        // 만약 새 씬에서 별도의 스폰 위치가 필요하다면 추가 코드를 작성하세요.
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
