using UnityEngine;

public class FireCauseTrigger : MonoBehaviour
{
    public string message = "🔥 화재 원인 발견: ";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 플레이어와 충돌 감지
        {
            ShowMessage();
        }
    }

    void ShowMessage()
    {
        Debug.Log(message + gameObject.name);  // 콘솔 출력
        UIManager.instance.ShowHint(message + gameObject.name);  // UI 출력 (추후 구현)
    }
}
