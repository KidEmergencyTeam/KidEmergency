using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLineAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SelectLineCoroutine());
    }

    private IEnumerator SelectLineCoroutine()
    {
        ExitLine line = FindObjectOfType<ExitLine>();
        line.ExitLineInteraction();
        
        while (!_isComplete)
        {
            if (line.isSelected)
            {
                _isComplete = true;
            }
            
            yield return null;
        }
    }
}
