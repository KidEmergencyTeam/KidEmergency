using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public DialogData testDialog;
    private DialogUI dialogUI;
    public Button testButton;

    private void Awake()
    {
        dialogUI = FindObjectOfType<DialogUI>();
        testButton.onClick.AddListener(DialogStart);
    }

    private void DialogStart()
    {
        dialogUI.dialogPanel.SetActive(true);
        StartCoroutine(TypingEffect(testDialog.dailogs));
    }
    
    private IEnumerator TypingEffect(string text)
    {
        dialogUI.dialogText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogUI.dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
}
