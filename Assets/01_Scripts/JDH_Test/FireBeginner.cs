using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBeginner : MonoBehaviour
{
    public enum PLACE{ CLASSROOM, HALLWAY };
    [Header("��� ����")]
    public PLACE place;
    [Header("NPC �� ��ȣ�ۿ� ������Ʈ")]
    public GameObject[] NPC;
    public FadeInOut fadeInOutImg;
    [Header("�����Ȳ üũ ����")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        fadeInOutImg.StartCoroutine(fadeInOutImg.FadeIn());
        //��� ����

        //��� ���� �� ��ư ������ ������ ���

        yield return null;
    }

}
