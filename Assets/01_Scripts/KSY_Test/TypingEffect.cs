using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class MultilineString
{
    // 입력 필드의 UI 크기를 조절
    // 11줄부터는 스크롤을 이용하여 확인
    [TextArea(1, 10)]
    [Header("타이핑 텍스트")]
    public string typingText;

    [Header("타이핑 오디오 클립")]
    public AudioClip typingSound;    

    [Header("버튼 설정")]
    public Button associatedButton;   

    [Header("타이핑 설정")]
    public float typingSpeed;   
}

public class TypingEffect : MonoBehaviour
{
    [Header("타이핑 설정")]
    public List<MultilineString> typingContents = new List<MultilineString>();  
    public TextMeshProUGUI tmpText;

    private float sentenceDelay = 1f;

    // 타이핑 진행 중 여부 플래그
    private bool isTyping = false;

    void Start()
    {
        // 각 MultilineString에 할당된 버튼에 클릭 이벤트 등록
        for (int i = 0; i < typingContents.Count; i++)
        {
            int index = i; // 클로저 문제 해결을 위해 지역 변수 사용
            if (typingContents[i].associatedButton != null)
            {
                typingContents[i].associatedButton.onClick.AddListener(() => PlayTypingAtIndex(index));
            }
        }
    }

    // 버튼 클릭 시 호출되어, 인덱스에 해당하는 텍스트를 타이핑 효과로 출력합니다.
    public void PlayTypingAtIndex(int index)
    {
        // 이미 타이핑 중이면 실행하지 않음
        if (isTyping)
            return;

        // 인덱스 범위 확인
        if (index < 0 || index >= typingContents.Count)
            return;

        // 텍스트 초기화 후 타이핑 시작
        tmpText.text = "";
        StartCoroutine(PlaySingleText(typingContents[index]));
    }

    IEnumerator PlaySingleText(MultilineString multiline)
    {
        isTyping = true;
        yield return StartCoroutine(TypeSentence(multiline.typingText, multiline.typingSound, multiline.typingSpeed));
        yield return new WaitForSeconds(sentenceDelay);
        isTyping = false;
    }

    // 한 문장을 한 글자씩 출력하며 타이핑 효과를 구현합니다.
    // 문장 시작 시 한 번 효과음을 재생합니다.
    IEnumerator TypeSentence(string sentence, AudioClip clip, float typingSpeed)
    {
        if (clip != null)
        {
            SoundManager.Instance.PlaySFX(clip.name, gameObject);
        }

        for (int i = 0; i < sentence.Length; i++)
        {
            tmpText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
