using UnityEngine;

public class SetiPosition : MonoBehaviour
{
    [Header("세티")]
    public GameObject seti;

    [Header("변경될 위치와 회전")]
    public Vector3 initialPosition;
    public Vector3 initialRotation;

    // 위치와 회전을 변경
    public void UpdatePosition()
    {
        if (seti != null)
        {
            // 위치 
            seti.transform.position = initialPosition;
            // 회전
            seti.transform.rotation = Quaternion.Euler(initialRotation);
            Debug.Log("세티 -> 위치 및 회전 변경 완료");
        }
        else
        {
            Debug.LogError("세티 -> null");
        }
    }
}
