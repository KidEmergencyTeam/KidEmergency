using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HoldLegAction : MonoBehaviour, IActionEffect
{
    private DeskLeg _leg;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;

    private void Awake()
    {
        _leg = FindObjectOfType<DeskLeg>();
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(Holding());
    }

    private IEnumerator Holding()
    {
        while (!_isComplete)
        {
            _leg.enabled = true;
            JSYNPCController npcCtrl = FindObjectOfType<JSYNPCController>();
            npcCtrl.SetNPCState("HoldDesk");
            if (_leg.IsHoldComplete())
            {
                _isComplete = true;
                _leg.RemoveOutline();
                _leg.enabled = false;
            }

            yield return null;
        }
    }
}
