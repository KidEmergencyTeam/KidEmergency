using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneSpawnInfo
{
    [Tooltip("씬 전환할 씬 이름입니다.")]
    public string sceneName;

    [Tooltip("해당 씬에서 사용할 스폰 위치입니다.")]
    public Vector3 spawnPosition;
}

public class WaitStateManager : MonoBehaviour
{
    public static WaitStateManager Instance;

    [Header("플레이어 관련")]
    public GameObject player;
    public TMP_Text statusUIText;

    [Header("현재 씬 기본 스폰 위치 (숫자 입력)")]
    [Tooltip("현재 씬에서 플레이어를 생성할 기본 위치입니다.")]
    public Vector3 spawnCoordinates = Vector3.zero;

    [Header("씬 전환 정보")]
    [Tooltip("준비 완료 후 씬 전환 시 사용할 씬 이름과 해당 씬에서의 스폰 위치입니다.")]
    public List<SceneSpawnInfo> sceneSpawnInfos;

    [Header("TMP 텍스트 태그 설정")]
    [Tooltip("TMP_Text 오브젝트를 찾을 때 사용할 태그입니다.")]
    [TagSelector] // 드롭다운으로 태그 선택 가능
    public string tmpTag;

    // 준비 완료 후 씬 전환 시 사용할 정보
    private string selectedSceneName = "";
    private Vector3 selectedSpawnPosition;

    private bool isReady = false;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    void Awake()
    {
        // 싱글톤 패턴 구현: 인스턴스가 없으면 자신을 할당하고, 이미 존재하면 파괴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (player != null)
            {
                DontDestroyOnLoad(player);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 현재 씬에서는 spawnCoordinates를 사용하여 플레이어 위치 지정
        if (player != null)
        {
            player.transform.position = spawnCoordinates;
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (!isReady)
        {
            // PC (시뮬레이션) 모드: 컨트롤러가 없으면 키보드 G키 체크
            if (!leftDevice.isValid && !rightDevice.isValid)
            {
                if (Input.GetKey(KeyCode.G))
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
        // statusUIText가 할당되어 있지 않을 경우 인스펙터에서 지정한 태그를 통해 TMP_Text 오브젝트 찾기
        if (statusUIText == null)
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(tmpTag);
            if (tmpObj != null)
            {
                statusUIText = tmpObj.GetComponent<TMP_Text>();
            }
            else
            {
                Debug.LogWarning("태그 '" + tmpTag + "'를 가진 TMP_Text 오브젝트를 찾을 수 없습니다.");
            }
        }

        if (statusUIText != null)
            statusUIText.text = status;
    }

    void StartGame()
    {
        Debug.Log("게임 시작!");

        // 준비 완료 후, 씬 전환 시 사용할 정보를 결정합니다.
        // sceneSpawnInfos가 있을 경우 랜덤 선택 (없으면 오류 처리)
        if (sceneSpawnInfos != null && sceneSpawnInfos.Count > 0)
        {
            int randomIndex = Random.Range(0, sceneSpawnInfos.Count);
            selectedSceneName = sceneSpawnInfos[randomIndex].sceneName;
            selectedSpawnPosition = sceneSpawnInfos[randomIndex].spawnPosition;
        }
        else
        {
            Debug.LogError("씬 전환 정보를 설정하세요.");
            return;
        }

        // 선택된 씬 이름으로 씬 전환
        if (!string.IsNullOrEmpty(selectedSceneName))
        {
            SceneManager.LoadScene(selectedSceneName);
        }
        else
        {
            Debug.LogError("선택된 sceneName이 없습니다.");
        }
    }

    // 새로운 씬 로드 후, 해당 씬에서 플레이어의 위치 초기화 및 TMP_Text 오브젝트 검색
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬에서 인스펙터에 설정한 태그(tmpTag)를 이용해 TMP_Text 찾기
        GameObject tmpObj = GameObject.FindGameObjectWithTag(tmpTag);
        if (tmpObj != null)
        {
            statusUIText = tmpObj.GetComponent<TMP_Text>();
            Debug.Log("새로운 씬에서 TMP_Text 할당됨.");
        }
        else
        {
            Debug.LogWarning("새로운 씬에서 태그 '" + tmpTag + "'를 가진 TMP_Text 오브젝트를 찾을 수 없습니다.");
        }

        // 플레이어의 위치 초기화
        if (player != null)
        {
            player.transform.position = selectedSpawnPosition;
            Debug.Log("새로운 씬에서 플레이어 위치 설정: " + selectedSpawnPosition);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
