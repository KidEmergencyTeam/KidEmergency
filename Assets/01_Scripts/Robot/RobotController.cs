using System.Collections;
using UnityEngine;

public class RobotController : SingletonManager<RobotController>
{
    public float speed = 1.0f;
    public Rob11ColorManager robotColorManager;
    public EmotionChanger emotionChanger;

    [Header("Repeat time for some animations")]
    public int playCount = 1; // Cyclyc Animations repeat time
    private int currentPlayCount = 0;
    private int currentNumber = 0; 
    int N = 2;             

    private string animationName = "YourAnimationName";
    public string pushableTag = "Pushable";
    
    int emo_i = 0;

    Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("speedMultiplier", speed);
    }

    public void SetAnimaiton(string animName)
    {
        anim.SetBool(animName, true);
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
        setEmotion(7);
    }

    // ^o^ 표정 ex) 선택지가 정답일 경우
    public void SetHappy()
    {
        SetAnimaiton("ThumbsUp");
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
}

