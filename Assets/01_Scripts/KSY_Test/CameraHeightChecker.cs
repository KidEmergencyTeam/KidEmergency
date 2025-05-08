using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class CameraHeightChecker : DisableableSingleton<CameraHeightChecker>
{
    // MainCamera 오브젝트 참조
    public GameObject vrCameraObj;

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
        FindCamera();
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

    void Update()
    {
        if (vrCameraObj == null)
        {
            return;
        }

        // 로컬 포지션 Y값 읽기
        float y = vrCameraObj.transform.localPosition.y;

        // step22_Flag -> true면 실행
        if (ScenarioManager.Instance.step22_Flag)
        {
            // 변경: -0.1 이하면 실행
            if (y <= -0.1f)
            {
                // 경고창 비활성화
                //UIManager.Instance.CloseWarningUI();

                // 콜백 실행 -> 플레이어 숙이기 완료
                HeightReached?.Invoke();
            }
            else
            {
                // 경고창 활성화
                //UIManager.Instance.OpenWarningUI();
            }
        }
    }
}
