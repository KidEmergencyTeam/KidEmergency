using System;
using UnityEngine;
using UnityEngine.UI;

public class EmergencyBellButton : MonoBehaviour
{
    [Header("EmergencyBell 버튼")]
    public Button emergencyBell;

    // 버튼 클릭 시 다른 스크립트에 알리기 위한 이벤트
    public static event Action OnEmergencyBellClicked;

    // 버튼 처리
    private void Start()
    {
        if (emergencyBell != null)
            emergencyBell.onClick.AddListener(HandleButtonClick);
        else
            Debug.LogError("EmergencyBell 버튼이 할당되지 않았습니다.");
    }

    // 버튼 클릭 시 실행
    private void HandleButtonClick()
    {
        OnEmergencyBellClicked?.Invoke();
    }
}
