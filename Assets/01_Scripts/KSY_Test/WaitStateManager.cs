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
public class SceneSpawnSettings
{
    [Header("씬 전환 정보 (첫 번째는 현재 씬)")]
    public List<SceneSpawnInfo> sceneSpawnInfos;
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

    // 현재 씬 포함 모든 씬의 spawn 정보를 하나의 리스트로 관리 (첫 번째 요소는 현재 씬)
    public SceneSpawnSettings sceneSpawnSettings;

    [Header("씬별 오브젝트 비활성화 설정")]
    public List<DisableSceneGroup> disableSceneGroups;

    // 준비 완료 후 씬 전환 시 사용할 정보
    private string selectedSceneName = "";
    private Vector3 selectedSpawnPosition;

    // 현재 씬은 0번, 씬 전환은 인덱스 1부터 시작하도록 함
    private int nextSceneIndex = 1;

    private bool isReady = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 초기 씬의 모든 오브젝트 초기화 완료 후, 플레이어 위치를 현재 씬의 spawn 위치(리스트의 첫 번째 요소)로 설정
    IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame();
        UpdateDisableSceneGroups(SceneManager.GetActiveScene().name);

        if (player != null && sceneSpawnSettings != null &&
            sceneSpawnSettings.sceneSpawnInfos != null &&
            sceneSpawnSettings.sceneSpawnInfos.Count > 0)
        {
            player.transform.position = sceneSpawnSettings.sceneSpawnInfos[0].spawnPosition;
        }
    }

    void Update()
    {
        if (!isReady)
        {
            // PC (시뮬레이션) 모드: 컨트롤러가 없으면 키보드 Space 체크
            if (!leftDevice.isValid && !rightDevice.isValid)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    isReady = true;
                    UpdateUI("준비 완료");
                    Invoke("StartGame", 2f);
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
                    isReady = true;
                    UpdateUI("준비 완료");
                    Invoke("StartGame", 2f);
                }
            }
        }
    }

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
        // 현재 씬(리스트 인덱스 0)을 제외한, 나머지 씬 정보를 사용하여 전환 (리스트에 최소 2개 이상의 정보가 있어야 함)
        if (sceneSpawnSettings != null &&
            sceneSpawnSettings.sceneSpawnInfos != null &&
            sceneSpawnSettings.sceneSpawnInfos.Count > 1)
        {
            selectedSceneName = sceneSpawnSettings.sceneSpawnInfos[nextSceneIndex].sceneName;
            selectedSpawnPosition = sceneSpawnSettings.sceneSpawnInfos[nextSceneIndex].spawnPosition;

            // 리스트 순환 (0은 현재 씬이므로 건너뛰기)
            nextSceneIndex = (nextSceneIndex + 1) % sceneSpawnSettings.sceneSpawnInfos.Count;
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
