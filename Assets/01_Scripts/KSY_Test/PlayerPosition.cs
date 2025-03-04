using UnityEngine;

// 플레이어의 초기 위치를 설정해주는 스크립트
public class PlayerPosition : MonoBehaviour
{
    public GameObject player;

    public Vector3 targetPosition;

    void Start()
    {
        if (player != null)
        {
            player.transform.position = targetPosition;
        }
        else
        {
            Debug.LogWarning("플레이어 오브젝트가 지정되지 않았습니다!");
        }
    }

    public void SetPlayerPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        if (player != null)
        {
            player.transform.position = targetPosition;
        }
        else
        {
            Debug.LogWarning("플레이어 오브젝트가 지정되지 않았습니다!");
        }
    }
}
