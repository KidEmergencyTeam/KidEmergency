using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class CameraHeightChecker : DisableableSingleton<CameraHeightChecker>
{
    [Header("MainCamera 오브젝트")]
    public GameObject vrCameraObj;

    [Header("WarningPopup")]
    public WarningPopup warningPopup;

    [Header("텍스트·이미지")]
    public WarningUIController warningUIController;

    [Header("숙이기 경고창 이미지")]
    public Sprite warningImage;

    [Header("숙이기 경고창 메시지")]
    [TextArea]
    public string heightWarningMessage;

    // 카메라 높이 도달 시 호출할 이벤트
    public event Action HeightReached;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 실행
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 메인 카메라 찾기
        FindCamera();

        // WarningPopup 스크립트 찾기
        CacheWarningPopupr();

        // WarningUIController 스크립트 찾기
        CacheWarningUIController();
    }

    // MainCamera 태그로 찾기
    private void FindCamera()
    {
        vrCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (vrCameraObj == null)
        {
            Debug.LogError("MainCamera 오브젝트를 찾을 수 없습니다.");
        }
    }

    // WarningPopup 스크립트 찾기
    private void CacheWarningPopupr()
    {
        warningPopup = FindObjectOfType<WarningPopup>();
        if (warningPopup == null)
        {
            Debug.LogError("WarningUIController.cs를 찾을 수 없습니다.");
        }
    }

    // WarningUIController 스크립트 찾기
    private void CacheWarningUIController()
    {
        warningUIController = FindObjectOfType<WarningUIController>();
        if (warningUIController == null)
        {
            Debug.LogError("WarningUIController.cs를 찾을 수 없습니다.");
        }
    }


    // 플레이어 높이 체크
    private void HeightCheck()
    {
        // 로컬 포지션 Y값 읽기
        float y = vrCameraObj.transform.localPosition.y;

        // step22_Flag -> true면 실행
        if (ScenarioManager.Instance.step22_Flag)
        {
            // 변경: -0.1 이하면 실행
            if (y <= -0.1f)
            {
                // 경고창 비활성화 -> WarningPopup.cs Inspector에서 설정한 이미지·텍스트로 변경
                warningPopup.warningUIController.SetWarning(warningImage, heightWarningMessage);

                // 경고창 비활성화 -> 여기서 조건을 하나 추가한다. -> 마스크 경고창 활성화 상태 체크
                //UIManager.Instance.CloseWarningUI();

                // 콜백 실행 -> 플레이어 숙이기 완료
                HeightReached?.Invoke();
            }
            else
            {
                // 경고창 활성화 -> Inspector에서 설정한 이미지·텍스트로 변경
                warningUIController.SetWarning(warningImage, heightWarningMessage);

                // 경고창 활성화 여기서 조건을 하나 추가한다. -> 마스크 경고창 활성화 상태 체크 -> 마스크 경고창이 활성화 된 상태라면 숙이고, 입을 가려라 -> 마스크 경고창이 비활성화 된 상태라면 숙이기만
                //UIManager.Instance.OpenWarningUI();
            }
        }
    }

    void Update()
    {
        if (vrCameraObj == null)
        {
            return;
        }

        // 플레이어 높이 체크
        HeightCheck();
    }
}
