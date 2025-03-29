using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCBAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private EqHomeTrigger box;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        StartCoroutine(OpenCircuitBox());
    }

    private IEnumerator OpenCircuitBox()
    {
        while (!_isComplete)
        {
            box.Corou();
            if (box.isOpen)
            {
                _isComplete = true;
            }

            yield return null;
        }
    }
    
}
