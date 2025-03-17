using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HoldingLegAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private DeskLeg leg;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(Holding());
    }

    private IEnumerator Holding()
    {
        while (!_isComplete)
        {
            leg.enabled = true;
            if (leg.IsHoldComplete())
            {
                _isComplete = true;
                leg.enabled = false;
            }

            yield return null;
        }
    }
}
