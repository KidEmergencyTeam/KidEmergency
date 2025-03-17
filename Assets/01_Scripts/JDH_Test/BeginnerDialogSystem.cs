using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeginnerDialogSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers; // Speaker�� Dialog �迭
    [SerializeField]
    private Dialog[] dialogs; // ���� �б��� ��� ��� �迭
    [SerializeField]
    private bool isAutoStart = true; // �ڵ� ���� ����
    [SerializeField]
    private float typingSpeed = 0.1f; // �ؽ�Ʈ Ÿ���� ȿ�� �ӵ�
    [SerializeField]
    private float dialogDelay = 2f; // ���� ���� �Ѿ�� �� ��� �ð�
    [SerializeField]
    private AudioSource audioSource; // ��� ����� ��¿�

    private int currentDialogIndex = -1; // ���� ��� ����
    private int currentDialogIndexNum = 0; // ���� ������ �ϴ� dialogIndex�� �迭 ����
    private bool isTypingEffect = false; // �ؽ�Ʈ Ÿ���� ȿ�� ��� ������ Ȯ���ϴ� ����
    public bool isDialogsEnd = false;  // ��ȭ�� ����Ǿ����� Ȯ���ϴ� ����

    private void Start()
    {
        InitializeSpeakers();
        if (isAutoStart)
        {
            StartCoroutine(AutoPlayDialog());
        }
    }

    // ����Ŀ �ʱ�ȭ (��ȭ UI�� ��Ȱ��ȭ)
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

    // �ڵ����� ��縦 �����ϴ� �ڷ�ƾ
    public IEnumerator AutoPlayDialog()
    {
        while (currentDialogIndex + 1 < dialogs.Length)
        {
            SetNextDialog();
            yield return new WaitForSeconds(dialogDelay + typingSpeed * dialogs[currentDialogIndexNum].dialogue.Length);
        }
        EndDialog();
    }

    // ���� ��� ����
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

    // UI ������Ʈ Ȱ��ȭ/��Ȱ��ȭ ����
    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.dialogImage.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
    }

    // Ÿ���� ȿ���� �����Ͽ� ��� ���
    private IEnumerator OnTypingText()
    {
        string fullText = dialogs[currentDialogIndexNum].dialogue;
        string displayedText = "";
        int charIndex = 0;
        isTypingEffect = true;

        PlayDialogAudio(); // Ÿ���� ���۰� ���ÿ� ����� ���

        while (charIndex < fullText.Length)
        {
            displayedText += fullText[charIndex];
            speakers[currentDialogIndexNum].textDialogue.text = displayedText;
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
    }

    // ��翡 �´� ������� ����ϴ� �Լ�
    private void PlayDialogAudio()
    {
        if (dialogs[currentDialogIndexNum].audioClip != null && audioSource != null)
        {
            audioSource.clip = dialogs[currentDialogIndexNum].audioClip;
            audioSource.Play();
        }
    }

    // ��ȭ ���� ó��
    public void EndDialog()
    {
        foreach (var speaker in speakers)
        {
            SetActiveObjects(speaker, false);
        }
        isDialogsEnd = true;
        Debug.Log("��� ��簡 �Ϸ�Ǿ����ϴ�.");
        this.gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct Speaker
{
    public Image dialogImage; // ��� â �̹���
    public TextMeshProUGUI textName; // ȭ���� �̸�
    public TextMeshProUGUI textDialogue; // ��� �ؽ�Ʈ
}

[System.Serializable]
public struct Dialog
{
    public int dialogIndex; // ��縦 ����� Speaker �ε���
    public string Name; // ȭ�� �̸�
    [TextArea(5, 5)]
    public string dialogue; // ��� ����
    public AudioClip audioClip; // �߰�: ��� ����� Ŭ��
}
