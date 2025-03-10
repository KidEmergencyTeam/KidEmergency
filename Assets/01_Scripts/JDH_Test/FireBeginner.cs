using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBeginner : MonoBehaviour
{
    public enum PLACE{ CLASSROOM, HALLWAY };
    [Header("장소 상태")]
    public PLACE place;
    [Header("NPC 및 상호작용 오브젝트")]
    public GameObject[] NPC;
    public FadeInOut fadeInOutImg;
    [Header("진행상황 체크 변수")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        fadeInOutImg.StartCoroutine(fadeInOutImg.FadeIn());
        //대사 시작

        //대사 종료 후 버튼 누르기 전까지 대기

        yield return null;
    }

}
