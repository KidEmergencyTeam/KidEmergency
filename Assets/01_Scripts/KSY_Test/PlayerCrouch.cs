using UnityEngine;
using System;

// 엎드리기 자세 -> 다음 스텝으로 전환
public class PlayerCrouch : MonoBehaviour
{
    // 플레이어 프리팹 -> 배
    public string targetTag = "Stomach";

    // 충돌 발생 시 다른 스크립트에 알리기 위한 이벤트
    public event Action OnStomachCollision;

    // 중복 실행 방지
    [SerializeField]
    private bool stepTriggered = false;

    // 콜라이더가 충돌할 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 1.충돌이 아직 발생하지 않았고
        // 2.충돌한 객체의 태그가 Stomach와 일치하면 실행
        if (!stepTriggered && other.CompareTag(targetTag))
        {
            // 중복 실행 방지
            stepTriggered = true;

            // 충돌 발생 시
            // ScenarioManager.cs -> Step22에서 호출
            OnStomachCollision?.Invoke();
            Debug.Log("머리와 배 콜라이더가 충돌했습니다.");

            // 중복 실행 체크 초기화
            stepTriggered = false;
        }
    }
}
