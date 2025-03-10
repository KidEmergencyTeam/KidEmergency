using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObjectAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        throw new System.NotImplementedException();
    }

    private void SetObject(DialogData dialogData)
    {
        for (int i = 0; i < dialogData.objects.Length; i++)
        {
            // dialogData.objects[i].AddComponent
        }
    }
}
