using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//손수건 손에서 놓지 못함. 다시 그랩되는 기능
public class HandkerHandler : MonoBehaviour
{
    public FireBeginner fireBeginner;
    [SerializeField] XRGrabInteractable interactable;
    [SerializeField] XRDirectInteractor leftHand;

    private void Awake()
    {
        interactable.selectExited.AddListener(CantGrabExit);
    }
    public void CantGrabExit(SelectExitEventArgs selectExit)
    {
        leftHand.interactionManager.SelectEnter((IXRSelectInteractor)leftHand,(IXRSelectInteractable)interactable);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fireBeginner.iscoverFace = true;
            Debug.Log("입과 코를 잘 가렸어~!");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireBeginner.iscoverFace = true;
            Debug.Log("입과 코를 잘 가렸어~!");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireBeginner.iscoverFace = false;
            Debug.Log("코와 입을 가려야해!");
        }
    }
}
