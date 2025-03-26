using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// �ռ����� �տ��� ���� ���ϰ� �ϰ�, FireBeginner�� EarthBeginner�� null���� ���ο� ���� �ٸ� �ൿ�� �ϵ��� ����
public class HandkerHandler : MonoBehaviour
{
    public FireBeginner fireBeginner;   // FireBeginner ����
    public EarthquakeBeginner earthBeginner;   // EarthBeginner ����
    [SerializeField] XRGrabInteractable interactable;  // Grab ������ ������Ʈ
    [SerializeField] XRDirectInteractor leftHand;  // ���� �� ���ͷ���

    private void Awake()
    {
        interactable.selectExited.AddListener(CantGrabExit);
    }

    // Grab�� ������ �� �ٽ� �׷��Ϸ��� ���
    public void CantGrabExit(SelectExitEventArgs selectExit)
    {
        leftHand.interactionManager.SelectEnter((IXRSelectInteractor)leftHand, (IXRSelectInteractable)interactable);
    }

    // Player�� �ռ����� ����� �� (fireBeginner�� earthBeginner ���� null ���ο� ���� �ٸ��� ó��)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner�� null�� �ƴϸ�, �԰� �ڸ� �� ������
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = true;
                Debug.Log("�԰� �ڸ� �� ���Ⱦ�~! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner�� null�̹Ƿ�, �԰� �ڸ� ���� �� �����ϴ�.");
            }

            // earthBeginner�� null�� �ƴϸ�, Ư�� ������ ����
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = true; 
                Debug.Log("���� ��� ����! (EarthBeginner)");
            }
            else
            {
                Debug.Log("EarthBeginner�� null�̹Ƿ�, ���� ��带 ������ �� �����ϴ�.");
            }
        }
    }

    // Player�� �ռ����� ��� ��� ���� �� (fireBeginner�� earthBeginner ���� null ���ο� ���� �ٸ��� ó��)
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner�� null�� �ƴϸ�, ��� �԰� �ڸ� �� ������
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = true;
                Debug.Log("�԰� �ڸ� �� ���Ⱦ�~! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner�� null�̹Ƿ�, �԰� �ڸ� ���� �� �����ϴ�.");
            }

            // earthBeginner�� null�� �ƴϸ�, Ư�� ������ ����
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = true;  //
                Debug.Log("�������� �Ӹ��� �� ��ȣ�ϰ� �ֱ���!!");
            }
            else
            {
                Debug.Log("EarthBeginner�� null�̹Ƿ�, �Ӹ��� ��ȣ�� �ʿ䰡 �����ϴ�.");
            }
        }
    }

    // Player�� �ռ����� ������ �� (fireBeginner�� earthBeginner ���� null ���ο� ���� �ٸ��� ó��)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // fireBeginner�� null�� �ƴϸ�, �԰� �ڸ� ������ ��Ȱ��ȭ
            if (fireBeginner != null)
            {
                fireBeginner.iscoverFace = false;
                Debug.Log("�ڿ� ���� ��������! (FireBeginner)");
            }
            else
            {
                Debug.Log("FireBeginner�� null�̹Ƿ�, �԰� �ڸ� ���� �� �����ϴ�.");
            }

            // earthBeginner�� null�� �ƴϸ�, Ư�� ������ ����
            if (earthBeginner != null)
            {
                earthBeginner.isprotectedHead = false;  // ����: ���� ��� ����
                Debug.Log("�������� �Ӹ��� �� ��ȣ�ؾ���!!");
            }
            else
            {
                Debug.Log("EarthBeginner�� null�̹Ƿ�, �Ӹ��� ��ȣ�� �ʿ䰡 �����ϴ�.");
            }
        }
    }
}
