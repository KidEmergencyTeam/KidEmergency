using UnityEngine;
using UnityEngine.UI;

public class EmergencyBell : MonoBehaviour
{
    public Button emergencyBell;

    private void Start()
    {
        if (emergencyBell != null)
        {
            // 버튼 클릭 시 경보벨 발생
            emergencyBell.onClick.AddListener(() =>
            {
                TypingEffect.Instance.StartContinuousSeparateTypingClip();
            });
        }
        else
        {
            Debug.LogWarning("emergencyBell이 할당되지 않았습니다.");
        }
    }
}
