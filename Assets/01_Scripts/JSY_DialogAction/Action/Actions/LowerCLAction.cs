using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerCLAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    public void StartAction()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator LowerCircuitLever()
    {
        while (!_isComplete)
        {
            
            yield return null;
        }
    }
}
