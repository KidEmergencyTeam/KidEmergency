using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneSpawnInfo
{
    [Header("씬 이름")]
    public string sceneName;

    [Header("해당 씬에서 사용할 스폰 위치")]
    public Vector3 spawnPosition;
}

[System.Serializable]
public class DisableSceneGroup
{
    [Header("씬 이름")]
    public string sceneName;

    [Header("해당 씬에서 비활성화할 오브젝트 (DontDestroyOnLoad 포함)")]
    public List<GameObject> objectsToDisable;
}

public class WaitStateManager : MonoBehaviour
{
    public static WaitStateManager Instance;

    [Header("플레이어")]
    public GameObject player;
    [TagSelector]
    public string playerTag;

    [Header("대기 상태 표시")]
    public TextMeshProUGUI WaitStateText;
    [TagSelector]
    public string WaitStateTextTag;

    [Header("씬 전환 정보")]
    public List<SceneSpawnInfo> sceneSpawnInfos;

    [Header("씬별 오브젝트 비활성화 설정")]
    public List<DisableSceneGroup> disableSceneGroups;

    // 준비 완료 후 씬 전환 시 사용할 정보
    private string selectedSceneName = "";
    private Vector3 selectedSpawnPosition;

    // 현재 씬은 0번, 씬 전환은 인덱스 1부터 시작하도록 함
    private int nextSceneIndex = 1;

    // 로컬 플레이어 준비 여부
    private bool isReady = false;

    // 게임 시작 대기 시간 (인스펙터에서 조정 가능)
    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    // 1명 기준이므로 준비된 인원 수를 확인할 필요 없이 단순히 1명일 때 준비 완료 처리
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (player == null && !string.IsNullOrEmpty(playerTag))
            {
                player = GameObject.FindGameObjectWithTag(playerTag);
            }
            if (player != null)
            {
                DontDestroyOnLoad(player);
            }
            if (WaitStateText != null)
            {
                DontDestroyOnLoad(WaitStateText.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(DelayedInitialization());

        // 초기 UI에 준비 상태를 표시 (1명 기준)
        UpdateUI("대기중: 0/1");

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 초기 씬의 모든 오브젝트 초기화 완료 후, 플레이어 위치를 현재 씬의 스폰 위치(리스트의 첫 번째 요소)로 설정
    IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame();
        UpdateDisableSceneGroups(SceneManager.GetActiveScene().name);

        if (player != null && sceneSpawnInfos != null && sceneSpawnInfos.Count > 0)
        {
            player.transform.position = sceneSpawnInfos[0].spawnPosition;
        }
    }

    void Update()
    {
        if (!isReady)
        {
            // PC (시뮬레이션) 모드: 컨트롤러가 없으면 키보드 Space 체크
            if (!leftDevice.isValid && !rightDevice.isValid)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RegisterReady();
                }
            }
            // VR 모드: 각 컨트롤러의 그립 버튼 입력 체크
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

    // 플레이어가 준비되었음을 등록하는 메서드 (1명 기준)
    void RegisterReady()
    {
        if (isReady) return; // 중복 등록 방지

        isReady = true;

        // 1명 기준이므로 바로 1/1 표시
        UpdateUI("준비 완료: 1/1");
        Debug.Log("플레이어 준비 완료! 게임 시작 대기...");

        Invoke("StartGame", gameStartDelay);
    }

    // UI 업데이트 (대기 상태 및 준비 상태 표시)
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

    void StartGame()
    {
        if (sceneSpawnInfos != null && sceneSpawnInfos.Count > 1)
        {
            selectedSceneName = sceneSpawnInfos[nextSceneIndex].sceneName;
            selectedSpawnPosition = sceneSpawnInfos[nextSceneIndex].spawnPosition;

            // 리스트 순환 (인덱스 0은 현재 씬이므로 건너뜀)
            nextSceneIndex = (nextSceneIndex + 1) % sceneSpawnInfos.Count;
            if (nextSceneIndex == 0)
                nextSceneIndex = 1;
        }
        else
        {
            Debug.LogError("씬 전환 정보를 설정하세요. (현재 씬 외에 추가 씬 정보가 필요합니다.)");
            return;
        }

        Debug.Log("게임 시작!");
        UpdateDisableSceneGroups(SceneManager.GetActiveScene().name);

        if (!string.IsNullOrEmpty(selectedSceneName))
        {
            SceneManager.LoadScene(selectedSceneName);
        }
        else
        {
            Debug.LogError("선택된 sceneName이 없습니다.");
        }
    }

    // 새로운 씬 로드 후, disableSceneGroups 업데이트 및 플레이어, UI 재설정
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateDisableSceneGroups(scene.name);

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

        if (player == null && !string.IsNullOrEmpty(playerTag))
        {
            player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                DontDestroyOnLoad(player);
        }

        if (player != null)
        {
            player.transform.position = selectedSpawnPosition;
        }
    }

    // 현재 씬 이름에 따라 disableSceneGroups에 등록된 오브젝트들을 활성/비활성 처리
    void UpdateDisableSceneGroups(string currentSceneName)
    {
        if (disableSceneGroups != null)
        {
            foreach (DisableSceneGroup group in disableSceneGroups)
            {
                if (group != null && group.objectsToDisable != null)
                {
                    bool shouldDisable = currentSceneName == group.sceneName;
                    foreach (GameObject obj in group.objectsToDisable)
                    {
                        if (obj != null)
                            obj.SetActive(!shouldDisable);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
