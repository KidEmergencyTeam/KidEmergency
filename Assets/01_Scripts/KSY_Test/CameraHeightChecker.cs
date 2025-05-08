using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class CameraHeightChecker : DisableableSingleton<CameraHeightChecker>
{
    [Header("FireEvacuationMask")]
    public FireEvacuationMask fireEvacuationMask;

    [Header("MainCamera 오브젝트")]
    public GameObject vrCameraObj;

    [Header("WarningPopup")]
    public WarningPopup warningPopup;

    [Header("텍스트·이미지")]
    public WarningUIController warningUIController;

    [Header("숙이기 경고창 이미지")]
    public Sprite warningImageA;

    [Header("숙이기 경고창 메시지")]
    [TextArea]
    public string heightWarningMessageA;

    [Header("숙이기 X + 손수건 입 X -> 이미지")]
    public Sprite warningImageB;

    [Header("숙이기 X + 손수건 입 X -> 메시지")]
    [TextArea]
    public string heightWarningMessageB;

    [Header("손수건 충돌 상태 여부")]
    public bool isHandkerOK = false;

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
        // FireEvacuationMask 이벤트 등록
        if (fireEvacuationMask != null)
        {
            fireEvacuationMask.OnHandkerchiefEnter += HandkerEnterCheck;
            fireEvacuationMask.OnHandkerchiefExit += HandkerExitCheck;
        }
        else
        {
            Debug.LogError("[WarningPopup] fireEvacuationMask -> null");
        }

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
        // Y값 읽기
        float y = vrCameraObj.transform.localPosition.y;

        // step22_Flag -> true면 실행(스텝22부터 조건만 맞으면 언제든지 실행 가능) 
        if (ScenarioManager.Instance.step22_Flag)
        {
            // 플레이어 높이가 -0.1 보다 같거나 작으면서(플레이어가 숙인 상태) isHandkerOK = true;면(손수건을 입에 붙여 놓은 상태) 실행
            if (y <= -0.1f && isHandkerOK)
            {
                // 경고창 비활성화 -> WarningPopup.cs Inspector에서 설정한 이미지·텍스트로 변경
                warningPopup.warningUIController.SetWarning(warningImageA, heightWarningMessageA);

                // 경고창 비활성화 -> 여기서 조건을 하나 추가한다. -> 마스크 경고창 활성화 상태 체크
                //UIManager.Instance.CloseWarningUI();

                // 콜백 실행 -> 플레이어 숙이기 완료
                HeightReached?.Invoke();
            }

            // 플레이어 높이가 -0.11 보다 같거나 크면서(플레이어가 숙이지 않은 상태) isHandkerOK = false면(손수건을 입에 떼어 놓은 상태) 실행
            if (y >= -0.11f && !isHandkerOK)
            {
                // 경고창 활성화 -> Inspector에서 설정한 이미지·텍스트로 변경
                warningUIController.SetWarning(warningImageA, heightWarningMessageA);

                // 경고창 활성화 여기서 조건을 하나 추가한다. -> 마스크 경고창 활성화 상태 체크 -> 마스크 경고창이 활성화 된 상태라면 숙이고, 입을 가려라 -> 마스크 경고창이 비활성화 된 상태라면 숙이기만
                //UIManager.Instance.OpenWarningUI();
            }
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandkerEnterCheck()
    {
        // 불함수를 넣어서 처리?
        isHandkerOK = true;
    }

    // 손수건과 충돌 종료할 때 실행
    private void HandkerExitCheck()
    {
        isHandkerOK = false;
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
