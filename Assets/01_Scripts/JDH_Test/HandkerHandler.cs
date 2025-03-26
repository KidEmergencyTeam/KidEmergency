using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// 손수건을 손에서 놓지 못하게 하고, FireBeginner와 EarthBeginner가 null인지 여부에 따라 다른 행동을 하도록 수정
public class HandkerHandler : MonoBehaviour
{
    public FireBeginner fireBeginner;   // FireBeginner 참조
    public EarthquakeBeginner earthBeginner;   // EarthBeginner 참조
    [SerializeField] XRGrabInteractable interactable;  // Grab 가능한 오브젝트
    [SerializeField] XRDirectInteractor leftHand;  // 좌측 손 인터랙터

    private void Awake()
    {
        interactable.selectExited.AddListener(CantGrabExit);
    }

    // Grab이 끝났을 때 다시 그랩하려는 기능
    public void CantGrabExit(SelectExitEventArgs selectExit)
    {
        leftHand.interactionManager.SelectEnter((IXRSelectInteractor)leftHand, (IXRSelectInteractable)interactable);
    }

    // Player가 손수건을 잡았을 때 (fireBeginner와 earthBeginner 각각 null 여부에 따라 다르게 처리)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner가 null이 아니면, 입과 코를 잘 가리기
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = true;
                Debug.Log("입과 코를 잘 가렸어~! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner가 null이므로, 입과 코를 가릴 수 없습니다.");
            }

            // earthBeginner가 null이 아니면, 특정 동작을 수행
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = true; 
                Debug.Log("지진 모드 시작! (EarthBeginner)");
            }
            else
            {
                Debug.Log("EarthBeginner가 null이므로, 지진 모드를 시작할 수 없습니다.");
            }
        }
    }

    // Player가 손수건을 계속 잡고 있을 때 (fireBeginner와 earthBeginner 각각 null 여부에 따라 다르게 처리)
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner가 null이 아니면, 계속 입과 코를 잘 가리기
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = true;
                Debug.Log("입과 코를 잘 가렸어~! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner가 null이므로, 입과 코를 가릴 수 없습니다.");
            }

            // earthBeginner가 null이 아니면, 특정 동작을 수행
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = true;  //
                Debug.Log("가방으로 머리를 잘 보호하고 있구나!!");
            }
            else
            {
                Debug.Log("EarthBeginner가 null이므로, 머리를 보호할 필요가 없습니다.");
            }
        }
    }

    // Player가 손수건을 놓았을 때 (fireBeginner와 earthBeginner 각각 null 여부에 따라 다르게 처리)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner가 null이 아니면, 입과 코를 가리기 비활성화
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = false;
                Debug.Log("코와 입을 가려야해! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner가 null이므로, 입과 코를 가릴 수 없습니다.");
            }

            // earthBeginner가 null이 아니면, 특정 동작을 수행
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = false;  // 예시: 지진 모드 종료
                Debug.Log("가방으로 머리를 잘 보호해야해!!");
            }
            else
            {
                Debug.Log("EarthBeginner가 null이므로, 머리를 보호할 필요가 없습니다.");
            }
        }
    }
}
