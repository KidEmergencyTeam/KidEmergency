using UnityEngine;

public class ShowOptionAction : MonoBehaviour, IActionEffect
{
    private bool isComplete;
    public bool IsActionComplete => isComplete;

    public void StartAction()
    {
        isComplete = false;
        SetOption();
    }
    
    private void SetOption()
    {
        for (int i = 0; i < ActionManager.Instance.currentDialog.choices.Length; i++)
        {
            if (UIManager.Instance.optionUI[i])
            {
                UIManager.Instance.SetOptionUI();
            }
        }
    }

    public void CompleteAction()
    {
        isComplete = true;
    }
}
