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

    [Header("타이핑 오디오 클립 1")]
    public AudioClip typingSound;

    [Header("타이핑 오디오 클립 2")]
    public AudioClip typingSound2;
}

public class TypingEffect : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static TypingEffect Instance { get; private set; }

    [Header("타이핑 설정")]
    public List<MultilineString> typingContents = new List<MultilineString>();

    [Header("타이핑 표시")]
    public TextMeshProUGUI typingText;

    [TagSelector]
    public string typingTextTag;

    [Header("타이핑 공통 속도")]
    public float commonTypingSpeed = 0.05f;

    // 대사 출력 전/후 대기 시간
    private float sentenceDelay = 0.5f;

    // 타이핑 진행 여부 
    private bool isTyping = false;

    // 시나리오 매니저에서 IsTyping을 통해 내부의 isTyping 값에 접근 가능
    // 따라서 시나리오 매니저에서 직접 값을 수정할 수 없음
    public bool IsTyping { get { return isTyping; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

    // 시나리오 매니저에서 인덱스에 해당하는 텍스트 타이핑 실행 및 사운드 재생
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
        
        // 타이핑과 사운드 재생을 함께 진행
        yield return StartCoroutine(TypeSentence(multiline.typingText, multiline.typingSound, multiline.typingSound2));
        
        // 타이핑 및 사운드 재생이 끝난 후 추가 대기(다음 대사 출력 전 대기 시간)
        yield return new WaitForSeconds(sentenceDelay);

        // 시나리오 매니저에서 해당 값을 받으면
        // 다음 코루틴으로 진행
        isTyping = false;
    }

    IEnumerator TypeSentence(string sentence, AudioClip clip1, AudioClip clip2)
    {
        float typingDuration = sentence.Length * commonTypingSpeed;
        float clipDuration1 = clip1 != null ? clip1.length : 0f;
        float clipDuration2 = clip2 != null ? clip2.length : 0f;
        float maxClipDuration = Mathf.Max(clipDuration1, clipDuration2);

        // 두 개의 오디오 클립을 동시에 재생 (SoundManager를 통해)
        if (clip1 != null)
        {
            SoundManager.Instance.PlaySFX(clip1.name, gameObject);
        }
        if (clip2 != null)
        {
            SoundManager.Instance.PlaySFX(clip2.name, gameObject);
        }

        // 한 글자씩 타이핑 효과 구현
        for (int i = 0; i < sentence.Length; i++)
        {
            if (typingText != null)
                typingText.text += sentence[i];
            yield return new WaitForSeconds(commonTypingSpeed);
        }

        // 텍스트 타이핑 동안 사운드가 아직 재생 중일 경우, 남은 시간만큼 추가 대기
        float remainingAudioTime = maxClipDuration - typingDuration;
        if (remainingAudioTime > 0f)
            yield return new WaitForSeconds(remainingAudioTime);
    }
}
