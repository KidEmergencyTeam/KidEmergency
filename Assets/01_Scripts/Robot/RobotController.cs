using System;
using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed = 1.0f;
    public Rob11ColorManager robotColorManager;
    public EmotionChanger emotionChanger;
    public Transform[] setiPos;
    
    private Animator _anim;
    
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetFloat("speedMultiplier", speed);
    }

    public void SetAnimaiton(string animName)
    {
        _anim.SetBool(animName, true);
    }
    
    // ㅁuㅁ 표정 ex) 기본 표정
    public void SetBasic()
    {
        setEmotion(0);
    }
    
    // \~/ 표정 ex) 선택지가 정답이 아닐 경우
    public void SetAngry()
    {
        SetAnimaiton("No");
        setEmotion(4);
    }

    // ^o^ 표정 ex) 선택지가 정답일 경우
    public void SetHappy()
    {
        SetAnimaiton("Thumb");
        setEmotion(1);
    }

    // 두리번 거리는 애니메이션 + >~< 표정 ex) 지진이 발생했을 경우
    public void SetLookingFor()
    {
        setEmotion(6);
    }
    
    public void setEmotion(int emoNumber)
    { 
        robotColorManager.ChangeBodyColor(emoNumber); 
        emotionChanger.SetEmotionEyes(emoNumber); 
        emotionChanger.SetEmotionMouth(emoNumber);
    }

    public void SetRobotPos(Transform parent)
    {
        this.transform.SetParent(parent);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}

