using UnityEngine;

// 세티 위치 
public class PositionInitializer : MonoBehaviour
{
    void Start()
    {
        // "Seti" 태그가 붙은 오브젝트 찾기
        GameObject seti = GameObject.FindGameObjectWithTag("Seti");
        if (seti == null)
        {
            Debug.LogError("Seti -> null");
        }
        else
        {
            // 위치 반영
            transform.position = seti.transform.position;
            // 회저 반영
            transform.rotation = seti.transform.rotation;
        }
    }
}
