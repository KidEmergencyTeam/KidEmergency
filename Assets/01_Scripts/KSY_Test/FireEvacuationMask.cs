using UnityEngine;
using System;

// 충돌 여부를 다른 스크립트에 전달하려는게 목적
public class FireEvacuationMask : MonoBehaviour
{
    [Header("충돌 대상 태그 (손수건)")]
    public string targetTag = "Handker";

    // 충돌 발생 시 다른 스크립트에 알리기 위한 이벤트
    public event Action OnHandkerchiefCollision;

    [Header("충돌 여부")]
    [SerializeField]
    private bool stepTriggered = false;

    // 콜라이더가 충돌할 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 1. 충돌이 아직 발생하지 않았고
        // 2. 충돌한 객체의 태그가 Handker와 일치하면 실행
        if (!stepTriggered && other.CompareTag(targetTag))
        {
            // 중복 실행 방지
            stepTriggered = true;

            // 충돌 발생 시 이벤트 호출 -> 다음 스텝으로 전환
            OnHandkerchiefCollision?.Invoke();

            // 중복 실행 체크 초기화
            stepTriggered = false;
        }
    }
}