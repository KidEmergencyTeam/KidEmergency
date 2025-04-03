using UnityEngine;

// 높이 측정
public class VRCameraHeightMonitor : MonoBehaviour
{
    [Header("메인 카메라")]
    public Transform mainCamera;

    // 메인 카메라 높이가
    // 목표 높이에 도달할 때만 디버그 출력
    [Header("목표 높이")]
    public float targetHeight = -0.2f; 

    void Update()
    {
        if (mainCamera != null)
        {
            float localHeight = mainCamera.localPosition.y;

            // 메인 카메라 높이가
            // 목표 높이와 같은지 확인
            if (Mathf.Approximately(localHeight, targetHeight))
            {
                Debug.Log("메인 카메라 높이: " + localHeight);
            }
        }
        else
        {
            Debug.LogError("메인 카메라 Transform이 할당되지 않았습니다!");
        }
    }
}
