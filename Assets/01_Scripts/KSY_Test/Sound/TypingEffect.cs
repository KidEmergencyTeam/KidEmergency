using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MultilineString
{
    [TextArea(1, 10)]
    [Header("타이핑 텍스트")]
    public string typingText;

    [Header("타이핑 오디오 클립")]
    public AudioClip typingSound;
}

public class TypingEffect : MonoBehaviour
{
    [Header("타이핑 설정")]
    public List<MultilineString> typingContents = new List<MultilineString>();

    [Header("타이핑 표시")]
    public TextMeshProUGUI typingText;

    [TagSelector]
    public string typingTextTag;

    [Header("타이핑 공통 속도")]
    public float commonTypingSpeed;

    [Header("타이핑 텍스트 삭제 대기 시간")]
    public float clearDelay;

    // 타이핑 전/후 대기 시간
    private float sentenceDelay = 0.5f;

    // 타이핑 진행 여부 (외부에서는 읽기 전용으로 접근)
    private bool isTyping = false;
    public bool IsTyping { get { return isTyping; } }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (typingText == null && !string.IsNullOrEmpty(typingTextTag))
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(typingTextTag);
            if (tmpObj != null)
                typingText = tmpObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(typingTextTag))
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(typingTextTag);
            if (tmpObj != null)
                typingText = tmpObj.GetComponent<TextMeshProUGUI>();
        }
    }

    // 외부에서 인덱스에 해당하는 텍스트 타이핑 실행
    public void PlayTypingAtIndex(int index)
    {
        if (isTyping)
        {
            Debug.Log("이미 타이핑 진행 중입니다.");
            return;
        }
        if (index < 0 || index >= typingContents.Count)
            return;

        if (typingText != null)
            typingText.text = "";
        StartCoroutine(PlaySingleText(typingContents[index]));
    }

    IEnumerator PlaySingleText(MultilineString multiline)
    {
        isTyping = true;
        yield return StartCoroutine(TypeSentence(multiline.typingText, multiline.typingSound));
        yield return new WaitForSeconds(sentenceDelay);
        yield return new WaitForSeconds(clearDelay);
        if (typingText != null)
            typingText.text = "";
        isTyping = false;
    }

    IEnumerator TypeSentence(string sentence, AudioClip clip)
    {
        if (clip != null)
        {
            // SoundManager는 별도 구현되어 있어야 합니다.
            SoundManager.Instance.PlaySFX(clip.name, gameObject);
        }

        for (int i = 0; i < sentence.Length; i++)
        {
            if (typingText != null)
                typingText.text += sentence[i];
            yield return new WaitForSeconds(commonTypingSpeed);
        }
    }
}
