using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MultilineString
{
    [TextArea(1, 10)]
    [Header("타이핑 텍스트")]
    public string typingText;

    [Header("타이핑 오디오 클립")]
    public AudioClip typingSound;

    [Header("버튼")]
    public Button associatedButton;
    [TagSelector]
    public string associatedButtonTag;

    [Header("타이핑 속도")]
    public float typingSpeed;
}

public class TypingEffect : MonoBehaviour
{
    [Header("타이핑 설정")]
    public List<MultilineString> typingContents = new List<MultilineString>();

    [Header("타이핑 표시")]
    public TextMeshProUGUI typingText;
    [TagSelector]
    public string typingTextTag;

    [Header("타이핑 텍스트 삭제 대기 시간")]
    public float clearDelay;

    // 타이핑 전,후 대기 시간
    private float sentenceDelay = 0.5f;

    // 타이핑 진행 중 여부 플래그
    private bool isTyping = false;

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
        // 씬 전환 시에도 참조가 유지되지 않을 수 있으므로, 최초에 할당되지 않았다면 태그로 찾아보기
        if (typingText == null && !string.IsNullOrEmpty(typingTextTag))
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(typingTextTag);
            if (tmpObj != null)
                typingText = tmpObj.GetComponent<TextMeshProUGUI>();
        }

        // 각 MultilineString에 할당된 버튼에 클릭 이벤트 등록
        for (int i = 0; i < typingContents.Count; i++)
        {
            // 반복문 내에서 버튼의 클릭 이벤트에 람다식을 추가할 때,
            // 람다식이 반복 변수(i)를 직접 참조하면 반복문 종료 후
            // 모든 람다식이 같은(i의 최종) 값을 참조하게 되는 문제가 발생할 수 있는
            // 문제를 방지
            int index = i;

            // 인스펙터에서 지정한 태그를 통해 버튼 컴포넌트 할당 (아직 할당되지 않았다면)
            if (typingContents[i].associatedButton == null &&
                !string.IsNullOrEmpty(typingContents[i].associatedButtonTag))
            {
                GameObject btnObj = GameObject.FindGameObjectWithTag(typingContents[i].associatedButtonTag);
                if (btnObj != null)
                    typingContents[i].associatedButton = btnObj.GetComponent<Button>();
                else
                    Debug.LogWarning("태그 '" + typingContents[i].associatedButtonTag + "'를 가진 버튼 오브젝트를 찾을 수 없습니다.");
            }

            if (typingContents[i].associatedButton != null)
            {
                typingContents[i].associatedButton.onClick.AddListener(() => PlayTypingAtIndex(index));
            }
        }
    }

    // 씬 전환 후 호출되는 콜백
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬에서 typingText를 태그를 통해 재참조
        if (!string.IsNullOrEmpty(typingTextTag))
        {
            GameObject tmpObj = GameObject.FindGameObjectWithTag(typingTextTag);
            if (tmpObj != null)
            {
                typingText = tmpObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("현재 씬에서 TextMeshProUGUI 할당됨.");
            }
            else
            {
                Debug.LogWarning("현재 씬에서 태그 '" + typingTextTag + "'를 가진 TextMeshProUGUI 오브젝트를 찾을 수 없습니다.");
            }
        }

        // 새 씬에서 각 버튼도 재참조가 필요할 경우, 반복문을 통해 재할당 (필요시)
        for (int i = 0; i < typingContents.Count; i++)
        {
            if (typingContents[i].associatedButton == null &&
                !string.IsNullOrEmpty(typingContents[i].associatedButtonTag))
            {
                GameObject btnObj = GameObject.FindGameObjectWithTag(typingContents[i].associatedButtonTag);
                if (btnObj != null)
                {
                    typingContents[i].associatedButton = btnObj.GetComponent<Button>();
                    Debug.Log("현재 씬에서 버튼(" + typingContents[i].associatedButtonTag + ") 할당됨.");
                }
                else
                {
                    Debug.LogWarning("현재 씬에서 태그 '" + typingContents[i].associatedButtonTag + "'를 가진 버튼 오브젝트를 찾을 수 없습니다.");
                }
            }
        }
    }

    // 버튼 클릭 시 호출되어, 인덱스에 해당하는 텍스트를 타이핑 효과로 출력합니다.
    public void PlayTypingAtIndex(int index)
    {
        if (isTyping)
        {
            Debug.Log("이미 타이핑이 진행 중입니다.");
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
        yield return StartCoroutine(TypeSentence(multiline.typingText, multiline.typingSound, multiline.typingSpeed));
        yield return new WaitForSeconds(sentenceDelay);
        yield return new WaitForSeconds(clearDelay);
        if (typingText != null)
            typingText.text = "";
        isTyping = false;
    }

    // 한 문장을 한 글자씩 출력하는 타이핑 효과 코루틴
    IEnumerator TypeSentence(string sentence, AudioClip clip, float typingSpeed)
    {
        if (clip != null)
        {
            SoundManager.Instance.PlaySFX(clip.name, gameObject);
        }

        for (int i = 0; i < sentence.Length; i++)
        {
            if (typingText != null)
                typingText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
