using UnityEngine;

public class ShowOptionAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete;
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
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
        _isComplete = true;
    }
}
