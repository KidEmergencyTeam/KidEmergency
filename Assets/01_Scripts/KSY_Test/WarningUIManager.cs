using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class WarningUIManager : DisableableSingleton<WarningUIManager>
{
    [Header("FireEvacuationMask")]
    public FireEvacuationMask fireEvacuationMask;

    [Header("Grabber")]
    public Grabber grabber;

    [Header("MainCamera 오브젝트")]
    public GameObject vrCameraObj;

    [Header("텍스트·이미지")]
    public WarningUIController warningUIController;

    [Header("숙이기 X + 손수건 O -> 이미지")]
    public Sprite warningImageA;

    [Header("숙이기 X + 손수건 O -> 메시지")]
    [TextArea]
    public string heightWarningMessageA;

    [Header("숙이기 X + 손수건 X -> 이미지")]
    public Sprite warningImageB;

    [Header("숙이기 X + 손수건 X -> 메시지")]
    [TextArea]
    public string heightWarningMessageB;

    [Header("숙이기 O + 손수건 X -> 이미지")]
    public Sprite warningImageC;

    [Header("숙이기 O + 손수건 X -> 메시지")]
    [TextArea]
    public string heightWarningMessageC;

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
        // FireEvacuationMask 스크립트 찾기
        CacheFireEvacuationMaskr();

        // FireEvacuationMask 이벤트 등록
        if (fireEvacuationMask != null)
        {
            fireEvacuationMask.OnHandkerchiefEnter += HandkerEnterCheck;
            fireEvacuationMask.OnHandkerchiefExit += HandkerExitCheck;
        }
        else
        {
            Debug.LogError("fireEvacuationMask -> null");
        }

        // Grabber 스크립트 찾기
        Grabber();

        // 메인 카메라 찾기
        FindCamera();

        // WarningUIController 스크립트 찾기
        CacheWarningUIController();
    }

    // FireEvacuationMask 스크립트 찾기
    private void CacheFireEvacuationMaskr()
    {
        fireEvacuationMask = FindObjectOfType<FireEvacuationMask>();
        if (fireEvacuationMask == null)
        {
            Debug.LogError("FireEvacuationMask.cs를 찾을 수 없습니다.");
        }
    }

    // Grabber 스크립트 찾기
    private void Grabber()
    {
        grabber = FindObjectOfType<Grabber>();
        if (grabber == null)
        {
            Debug.LogError("Grabber.cs를 찾을 수 없습니다.");
        }
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

    // WarningUIController 스크립트 찾기
    private void CacheWarningUIController()
    {
        warningUIController = FindObjectOfType<WarningUIController>();
        if (warningUIController == null)
        {
            Debug.LogError("WarningUIController.cs를 찾을 수 없습니다.");
        }
    }

    // 스텝 21까지 적용되는 경고창 실행 흐름
    private void BasicWarningUI()
    {
        // step22_Flag -> true면 중단하고 false면 정상 진행
        if (ScenarioManager.Instance.step22_Flag)
        {
            return;
        }

        // 1) 손수건 잡은 상태에서 손수건 OK(true) → 경고창 끄기
        if (grabber.currentGrabbedObject != null && isHandkerOK)
        {
            UIManager.Instance.CloseWarningUI();
        }
        // 2) 손수건 잡은 상태에서 손수건 NOT OK(false) → 손수건 경고
        else if(grabber.currentGrabbedObject != null && !isHandkerOK)
        {
            warningUIController.SetWarning(warningImageC, heightWarningMessageC);
            UIManager.Instance.OpenWarningUI();
        }
    }

    // 스텝 22부터 적용되는 경고창 실행 흐름
    private void Step22WarningUI()
    {
        // Y값 읽기
        float y = vrCameraObj.transform.localPosition.y;

        // step22_Flag -> false면 중단하고 true면 정상 진행
        if (!ScenarioManager.Instance.step22_Flag)
        {
            return;
        }

        // 플레이어 숙였는지를 판단하는 기준값
        const float bendThreshold = -0.1f;    

        // 머리 숙이기 완료 콜백 -> 다음 단계 진행 가능, 추후에 한 번만 호출하고 중단하도록 구현
        if (y <= bendThreshold)
        {
            HeightReached?.Invoke();
        }

        // 1) 숙이고(isHandkerOK), 손수건 OK(true) → 경고창 끄기
        if (y <= bendThreshold && isHandkerOK)
        {
            UIManager.Instance.CloseWarningUI();
        }
        // 2) 숙이고(isHandkerOK), 손수건 NOT OK(false) → 손수건만 경고
        else if (y <= bendThreshold && !isHandkerOK)
        {
            warningUIController.SetWarning(warningImageC, heightWarningMessageC);
            UIManager.Instance.OpenWarningUI();
        }
        // 3) 안 숙이고(!isHandkerOK), 손수건 OK(true) → 숙이기만 경고
        else if (y > bendThreshold && isHandkerOK)
        {
            warningUIController.SetWarning(warningImageA, heightWarningMessageA);
            UIManager.Instance.OpenWarningUI();
        }
        // 4) 안 숙이고(!isHandkerOK), 손수건 NOT OK(false) → 전부 경고
        else
        {
            warningUIController.SetWarning(warningImageB, heightWarningMessageB);
            UIManager.Instance.OpenWarningUI();
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandkerEnterCheck()
    {
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

        // 스텝 21까지 적용되는 경고창 실행 흐름
        BasicWarningUI();

        // 스텝 22부터 적용되는 경고창 실행 흐름
        Step22WarningUI();
    }
}
