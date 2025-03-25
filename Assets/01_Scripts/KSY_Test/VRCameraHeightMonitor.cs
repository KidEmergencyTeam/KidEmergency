using UnityEngine;

// 메인 카메라 높이 값 출력
public class VRCameraHeightMonitor : MonoBehaviour
{
    public Transform mainCamera;

    void Update()
    {
        if (mainCamera != null)
        {
            float localHeight = mainCamera.localPosition.y;
            Debug.Log("메인 카메라 높이: " + localHeight);
        }
        else
        {
            Debug.LogError("메인 카메라 Transform이 할당되지 않았습니다!");
        }
    }
}
