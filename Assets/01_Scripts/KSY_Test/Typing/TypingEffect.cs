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

public class TypingEffect : DisableableSingleton<TypingEffect>
{
    [Header("타이핑 설정")]
    public List<MultilineString> typingContents = new List<MultilineString>();

    [Header("타이핑 표시")]
    public TextMeshProUGUI typingText;

    [TagSelector]
    public string typingTextTag;

    [Header("타이핑 공통 속도")]
    public float commonTypingSpeed = 0.14f;

    // 타이핑 진행 시 별도로 재생할 오디오 클립 
    [Header("화재 경보벨")]
    public AudioClip separateTypingClip;

    // 연속 재생용 AudioSource 
    private AudioSource continuousAudioSource;

    // 타이핑 진행 여부 
    private bool isTyping = false;

    // 시나리오 매니저에서 IsTyping을 통해 내부의 isTyping 값에 접근 가능
    // 따라서 시나리오 매니저 내의 스텝별 코루틴에서 해당 값을 받으면 다음 스텝으로 진행
    public bool IsTyping { get { return isTyping; } }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(typingTextTag))
        {
            // 비활성화된 오브젝트도 모두 포함하여 찾기 위해 Resources.FindObjectsOfTypeAll를 사용
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // 태그가 일치하는지 확인 (비활성화된 오브젝트도 포함)
                if (obj.CompareTag(typingTextTag))
                {
                    typingText = obj.GetComponent<TextMeshProUGUI>();
                    if (typingText != null)
                        break;
                }
            }
        }
    }

    // 인덱스에 해당하는 텍스트 타이핑 실행 및 사운드 재생
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

        // 타이핑 효과와 사운드를 동시에 시작하고, 모두 완료될 때까지 기다림
        yield return StartCoroutine(TypeSentence(multiline.typingText, multiline.typingSound, multiline.typingSound2));

        // 타이핑이 끝나면 다음 단계로 진행할 수 있도록 isTyping 값을 false로 전환
        // 따라서 시나리오 매니저 내의 스텝별 코루틴에서 해당 값을 받으면 다음 스텝으로 진행
        isTyping = false;
    }

    IEnumerator TypeSentence(string sentence, AudioClip clip1, AudioClip clip2)
    {
        float typingDuration = sentence.Length * commonTypingSpeed;
        float clipDuration1 = clip1 != null ? clip1.length : 0f;
        float clipDuration2 = clip2 != null ? clip2.length : 0f;
        float maxClipDuration = Mathf.Max(clipDuration1, clipDuration2);

        // 리스트 내의 오디오 클립 재생
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

        // 텍스트 타이핑 동안 재생된 사운드가 아직 끝나지 않았다면 남은 시간만큼 추가 대기
        float remainingAudioTime = maxClipDuration - typingDuration;
        if (remainingAudioTime > 0f)
            yield return new WaitForSeconds(remainingAudioTime);
    }

    // 화재 경보벨 연속 재생 시작 (Step18에서 호출)
    public void StartContinuousSeparateTypingClip()
    {
        if (separateTypingClip == null)
            return;

        if (continuousAudioSource == null)
        {
            continuousAudioSource = gameObject.AddComponent<AudioSource>();
            continuousAudioSource.clip = separateTypingClip;
            continuousAudioSource.loop = true;
            continuousAudioSource.playOnAwake = false;
        }

        if (!continuousAudioSource.isPlaying)
        {
            continuousAudioSource.Play();
            Debug.Log("화재 경보벨 연속 재생 시작");
        }
    }

    // 화재 경보벨 연속 재생 종료 (Step36에서 호출)
    public void StopContinuousSeparateTypingClip()
    {
        if (continuousAudioSource != null && continuousAudioSource.isPlaying)
        {
            continuousAudioSource.Stop();
            Debug.Log("화재 경보벨 연속 재생 종료");
        }
    }
}
