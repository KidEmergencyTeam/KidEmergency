using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//�ռ��� �տ��� ���� ����. �ٽ� �׷��Ǵ� ���
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
            Debug.Log("�԰� �ڸ� �� ���Ⱦ�~!");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireBeginner.iscoverFace = true;
            Debug.Log("�԰� �ڸ� �� ���Ⱦ�~!");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fireBeginner.iscoverFace = false;
            Debug.Log("�ڿ� ���� ��������!");
        }
    }
}
