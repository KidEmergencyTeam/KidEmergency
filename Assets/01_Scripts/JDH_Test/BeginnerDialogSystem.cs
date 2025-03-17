using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeginnerDialogSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers; // Speaker의 Dialog 배열
    [SerializeField]
    private Dialog[] dialogs; // 현재 분기의 대사 목록 배열
    [SerializeField]
    private bool isAutoStart = true; // 자동 시작 여부
    [SerializeField]
    private float typingSpeed = 0.1f; // 텍스트 타이핑 효과 속도
    [SerializeField]
    private float dialogDelay = 2f; // 다음 대사로 넘어가기 전 대기 시간
    [SerializeField]
    private AudioSource audioSource; // 대사 오디오 출력용

    private int currentDialogIndex = -1; // 현재 대사 순번
    private int currentDialogIndexNum = 0; // 현재 설명을 하는 dialogIndex의 배열 순번
    private bool isTypingEffect = false; // 텍스트 타이핑 효과 재생 중인지 확인하는 변수
    public bool isDialogsEnd = false;  // 대화가 종료되었는지 확인하는 변수

    private void Start()
    {
        InitializeSpeakers();
        if (isAutoStart)
        {
            StartCoroutine(AutoPlayDialog());
        }
    }

    // 스피커 초기화 (대화 UI를 비활성화)
    private void InitializeSpeakers()
    {
        foreach (var speaker in speakers)
        {
            speaker.dialogImage.gameObject.SetActive(false);
            speaker.textName.gameObject.SetActive(false);
            speaker.textDialogue.gameObject.SetActive(false);
            speaker.textName.text = "";
            speaker.textDialogue.text = "";
        }
        currentDialogIndex = -1;
        isDialogsEnd = false;
    }

    // 자동으로 대사를 실행하는 코루틴
    public IEnumerator AutoPlayDialog()
    {
        while (currentDialogIndex + 1 < dialogs.Length)
        {
            SetNextDialog();
            yield return new WaitForSeconds(dialogDelay + typingSpeed * dialogs[currentDialogIndexNum].dialogue.Length);
        }
        EndDialog();
    }

    // 다음 대사 설정
    private void SetNextDialog()
    {
        if (currentDialogIndex >= 0)
        {
            SetActiveObjects(speakers[currentDialogIndexNum], false);
        }

        currentDialogIndex++;
        currentDialogIndexNum = dialogs[currentDialogIndex].dialogIndex;
        SetActiveObjects(speakers[currentDialogIndexNum], true);
        speakers[currentDialogIndexNum].textName.text = dialogs[currentDialogIndex].Name;

        StartCoroutine(OnTypingText());
    }

    // UI 오브젝트 활성화/비활성화 설정
    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.dialogImage.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
    }

    // 타이핑 효과를 적용하여 대사 출력
    private IEnumerator OnTypingText()
    {
        string fullText = dialogs[currentDialogIndexNum].dialogue;
        string displayedText = "";
        int charIndex = 0;
        isTypingEffect = true;

        PlayDialogAudio(); // 타이핑 시작과 동시에 오디오 재생

        while (charIndex < fullText.Length)
        {
            displayedText += fullText[charIndex];
            speakers[currentDialogIndexNum].textDialogue.text = displayedText;
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
    }

    // 대사에 맞는 오디오를 재생하는 함수
    private void PlayDialogAudio()
    {
        if (dialogs[currentDialogIndexNum].audioClip != null && audioSource != null)
        {
            audioSource.clip = dialogs[currentDialogIndexNum].audioClip;
            audioSource.Play();
        }
    }

    // 대화 종료 처리
    public void EndDialog()
    {
        foreach (var speaker in speakers)
        {
            SetActiveObjects(speaker, false);
        }
        isDialogsEnd = true;
        Debug.Log("모든 대사가 완료되었습니다.");
        this.gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct Speaker
{
    public Image dialogImage; // 대사 창 이미지
    public TextMeshProUGUI textName; // 화자의 이름
    public TextMeshProUGUI textDialogue; // 대사 텍스트
}

[System.Serializable]
public struct Dialog
{
    public int dialogIndex; // 대사를 출력할 Speaker 인덱스
    public string Name; // 화자 이름
    [TextArea(5, 5)]
    public string dialogue; // 대사 내용
    public AudioClip audioClip; // 추가: 대사 오디오 클립
}
