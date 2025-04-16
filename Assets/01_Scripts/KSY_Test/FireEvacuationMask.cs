using UnityEngine;
using System;

// 충돌 여부를 다른 스크립트에 전달하려는게 목적 -> 충돌 여부에 따라 처리
public class FireEvacuationMask : MonoBehaviour
{
    [Header("충돌 대상 태그 (손수건)")]
    public string targetTag = "Handker";

    [Header("입 콜라이더")]
    public Collider myCollider;

    [Header("Grabber")]
    public Grabber myGrabber;

    // 손수건과 충돌할 때 다른 스크립트에 알리기 위한 이벤트
    public event Action OnHandkerchiefEnter;

    // 손수건과 충돌 종료할 때 다른 스크립트에 알리기 위한 이벤트
    public event Action OnHandkerchiefExit;

    private void Update()
    {
        EnableTriggerMode();
    }

    // Trigger 켜기 및 끄기
    public void EnableTriggerMode()
    {
        // 손수건을 잡고 있다면 트리거 켜기
        if(myGrabber.currentGrabbedObject != null)
        {
            myCollider.isTrigger = true;
        }

        // 아니라면 트리거 끄기
        else
        {
            myCollider.isTrigger = false;
        }
    }

    // 콜라이더가 충돌할 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 태그가 Handker와 일치하면 실행
        if (other.CompareTag(targetTag))
        {
            // 1.다음 시나리오 스텝으로 전환
            // 2.활성화 상태인 경고창 ui를 비활성화
            OnHandkerchiefEnter?.Invoke();
        }
    }

    // 콜라이더가 충돌 종료할 때 호출
    private void OnTriggerExit(Collider other)
    {
        // 충돌 종료한 객체의 태그가 Handker와 일치하면 실행
        if (other.CompareTag(targetTag))
        {
            // 비활성화 상태인 경고창 ui를 활성화
            OnHandkerchiefExit?.Invoke();
        }
    }
}

// 콜라이더가 처음에 비활성화라 -> 잡았을때만 활성화 상태 전환