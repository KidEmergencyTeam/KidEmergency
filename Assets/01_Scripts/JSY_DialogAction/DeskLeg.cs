using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeskLeg : MonoBehaviour
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;

    private float _durationTime = 0f; // 지속 시간
    private float _endTime = 5f; // 종료 시간
    public bool isHoldComplete = false;
    
    private void Start()
    {
        this.enabled = false;
    }

    private void Update()
    {
        Holding();
    }

    public void Holding()
    {
        bool isLeftGrapped = _leftController.selectAction.action.ReadValue<float>() > 0;
        bool isRightGrapped = _rightController.selectAction.action.ReadValue<float>() > 0;
        bool isInteractable = Vector3.Distance(this.transform.position, _leftController.transform.position) < 0.2f && isLeftGrapped ||
                                  Vector3.Distance(this.transform.position, _rightController.transform.position) < 0.2f && isRightGrapped;
        if (isInteractable)
        { 
            UIManager.Instance.CloseWarningUI();
            
            _durationTime += Time.deltaTime;
            if (_durationTime >= _endTime)
            { 
                isHoldComplete = true;
            }
        }
        
        else
        {
            _durationTime = 0f;
            UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
            UIManager.Instance.OpenWarningUI(); 
            isHoldComplete = false;
        }
    }
    
    public bool IsHoldComplete()
    {
        if (isHoldComplete)
        {
            return true;
        }
        
        else return false;
    }
}
