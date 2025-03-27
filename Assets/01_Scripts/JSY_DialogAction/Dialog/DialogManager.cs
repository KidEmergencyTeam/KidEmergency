using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : SingletonManager<DialogManager>
{
    private AudioSource _dialogAudio;

    protected override void Awake()
    {
        base.Awake();
        _dialogAudio = GetComponent<AudioSource>();
    }

    public void DialogStart() // 첫 장면이 시작될 때 사용되는 메서드
    {
        StartCoroutine(ShowDialog());
    }

    public IEnumerator ShowDialog()
    {
        RobotController robot = FindObjectOfType<RobotController>();
        
        UIManager.Instance.dialogUI.dialogPanel.SetActive(true);

        for (int i = 0; i < ActionManager.Instance.currentDialog.dialogs.Length; i++)
        {
            string dialog = ActionManager.Instance.currentDialog.dialogs[i];
            AudioClip audio = ActionManager.Instance.currentDialog.audios[i];
            yield return StartCoroutine(TypingEffect(dialog, audio));
        }

        UIManager.Instance.dialogUI.dialogPanel.SetActive(false);
        robot.SetBasic();
        
        if (ActionManager.Instance.currentDialog.choices.Length > 0)
        {
            ActionManager.Instance.currentAction = ActionType.ShowOption;
            ActionManager.Instance.UpdateAction();
        }
        else
        {
            ActionManager.Instance.currentAction = ActionManager.Instance.currentDialog.nextActionType;
            ActionManager.Instance.beforeDialog = ActionManager.Instance.currentDialog;
            ActionManager.Instance.currentDialog = ActionManager.Instance.currentDialog.nextDialog;   
            ActionManager.Instance.UpdateAction();
        }
    }
    
    private IEnumerator TypingEffect(string text, AudioClip audio)
    {
        UIManager.Instance.dialogUI.dialogText.text = "";

        if (audio != null)
        {
            _dialogAudio.clip = audio;
            _dialogAudio.Play(); 
        }
        
        foreach (char letter in text.ToCharArray())
        {
            UIManager.Instance.dialogUI.dialogText.text += letter;
            yield return new WaitForSeconds(0.125f);
        }
        
        yield return new WaitUntil(() => UIManager.Instance.dialogUI.dialogText.text == text && !_dialogAudio.isPlaying);
        yield return new WaitForSeconds(1f);
    }
    
}