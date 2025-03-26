using UnityEngine;

// 비활성화된 손수건을 활성화 시켜주는 스크립트
// 다른 스크립트에서 ActivateHandkerObject 함수를 불러와서 실행
public class HandkerActivation : MonoBehaviour
{
    // 손수건 할당
    public GameObject handkerObject;

    // 비활성화된 객체를 활성화
    public void ActivateHandkerObject()
    {
        if (handkerObject != null)
        {
            handkerObject.SetActive(true);
            Debug.Log("손수건 활성화");
        }
        else
        {
            Debug.LogError("손수건 없음");
        }
    }
}
