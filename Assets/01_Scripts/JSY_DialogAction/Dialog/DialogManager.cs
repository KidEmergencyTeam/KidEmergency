using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : SingletonManager<DialogManager>
{
    public Button testButton;

    protected override void Awake()
    {
        base.Awake();       
        testButton.onClick.AddListener(DialogStart);
    }

    private void DialogStart()
    {
        StartCoroutine(ShowDialog());
    }

    public IEnumerator ShowDialog()
    {
        UIManager.Instance.dialogUI.dialogPanel.SetActive(true); 

        foreach (string dialog in ActionManager.Instance.currentDialog.dialogs)
        {
            yield return StartCoroutine(TypingEffect(dialog));
        }
        
        UIManager.Instance.dialogUI.dialogPanel.SetActive(false);
        
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
    
    private IEnumerator TypingEffect(string text)
    {
        UIManager.Instance.dialogUI.dialogText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            UIManager.Instance.dialogUI.dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitUntil(() => UIManager.Instance.dialogUI.dialogText.text == text);
        yield return new WaitForSeconds(1f);
    }
    
}