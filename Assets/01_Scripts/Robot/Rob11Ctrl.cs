using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rob11Ctrl : MonoBehaviour
{
    public float speed = 1.0f;

    public GameObject MouthEmo, MouthSpeech;

    public Rob11ColorManager robotColorManager;
    public EmotionChanger emotionChanger;

    [Header("Repeat time for some animations")]
    public int playCount = 1; // Cyclyc Animations repeat time
    private int currentPlayCount = 0;

    private int currentNumber = 0; //
    int N = 2;             

    private string animationName = "YourAnimationName";
    public string pushableTag = "Pushable";


    int emo_i = 0;

    Animator anim;

    /*
     * Emotions list with ID
         0.Neutral
         1.Happy
         2.Sad
         3.Distrust
         4.Wonder
         5.Death
         6.Disgust
         7.Evil
         8.Cry
         9.Love
     */
    
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("speedMultiplier", speed);
    }

    void Update()
    {
//---------------------------------------- EMOTIONS -----------------

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            resetEmo();

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetBool("Angry", true);
            setEmotion(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetBool("Cry", true);
            setEmotion(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetBool("Thumb", true);
            setEmotion(9);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            anim.SetBool("Win", true);
            setEmotion(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetBool("DontKnow", true);
            setEmotion(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            anim.SetBool("Hello", true);
            setEmotion(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            anim.SetBool("Laught", true);
            setEmotion(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            anim.SetBool("LookingFor", true);
            setEmotion(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            animationName = "Dance0";
            robotColorManager.isRainbowCycles = true;
            setEmotion(1);
            StartCoroutine(PlayAnimationMultipleTimes());
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            animationName = "Dance1";
            robotColorManager.isRainbowCycles = true;
            setEmotion(1);
            StartCoroutine(PlayAnimationMultipleTimes());
        }
    }

    public void setEmotion(int emoNumber)
    { 
        robotColorManager.ChangeBodyColor(emoNumber); 
        emotionChanger.SetEmotionEyes(emoNumber); 
        emotionChanger.SetEmotionMouth(emoNumber);
    }

    IEnumerator PlayAnimationMultipleTimes()
    {
        for (int i = 0; i < playCount; i++)
        {
            anim.SetBool(animationName, true);
            yield return new WaitForSeconds(playCount);
        }
        
        anim.SetBool(animationName, false);
        robotColorManager.isRainbowCycles = false;
        anim.SetBool("reset", true);
        resetEmo();
        Debug.Log("Animation Done");
    }
    
    void resetEmo()
    {
        setEmotion(0);
        anim.SetBool("reset", true);
    }

}

