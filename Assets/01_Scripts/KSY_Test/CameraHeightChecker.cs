using UnityEngine;
using System;

public class CameraHeightChecker : DisableableSingleton<CameraHeightChecker>
{
    // MainCamera 오브젝트 참조
    private GameObject vrCameraObj;

    // 카메라 높이 도달 시 호출할 이벤트
    public event Action HeightReached;

    void Start()
    {
        // MainCamera 태그가 붙은 오브젝트 찾기
        vrCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (vrCameraObj == null)
        {
            Debug.LogError("MainCamera 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 체크 중이 아니거나 카메라가 없으면 리턴
        if (vrCameraObj == null)
        {
            return;
        }

        // 로컬 포지션 Y값 읽기
        float y = vrCameraObj.transform.localPosition.y;

        // -0.3 이상, -0.2 이하 범위에 도달했는지 확인
        if (y >= -0.3f && y <= -0.2f)
        {
            // 콜백 실행 -> 플레이어 숙이기 완료
            HeightReached?.Invoke();
        }
    }
}
