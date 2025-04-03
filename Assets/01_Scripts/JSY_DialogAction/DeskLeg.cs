using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeskLeg : MonoBehaviour
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private GameObject[] _legs;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;
    
    private HandAnimation _handAnimation;
    private ActionBasedController _leftController;
    private ActionBasedController _rightController;
    private float _durationTime = 0f; // 지속 시간
    private float _endTime = 5f; // 종료 시간
    private bool _isTrigger = false;
    public bool isHoldComplete = false;

    private void Awake()
    {
        for (int i = 0; i < _legs.Length; i++)
        {
            _legs[i].GetComponent<Outlinable>().enabled = false;
            _legs[i].GetComponent<BaseOutlineObject>().enabled = false;
        }
        
        _leftController = GameObject.Find("Left Controller").GetComponent<ActionBasedController>();
        _rightController = GameObject.Find("Right Controller").GetComponent<ActionBasedController>();
    }

    private void Start()
    {
        _handAnimation = FindObjectOfType<HandAnimation>();
        this.enabled = false;
    }

    private void Update()
    {
        Holding();
    }

    public void Holding()
    {
        bool isLeftGrapped = _leftController.selectAction.action.ReadValue<float>() > 0;
        if(isLeftGrapped) _handAnimation.animator.SetFloat("Left Trigger", 1);
        bool isRightGrapped = _rightController.selectAction.action.ReadValue<float>() > 0;
        if(isRightGrapped) _handAnimation.animator.SetFloat("Right Trigger", 1);
        // bool isInteractable =
        //     Vector3.Distance(_legs[0].transform.position, _leftHand.transform.position) < 0.05f && isLeftGrapped;
        bool isInteractable = Vector3.Distance(_legs[0].transform.position, _leftController.transform.position) < 0.05f && isLeftGrapped &&
                                  Vector3.Distance(_legs[1].transform.position, _rightController.transform.position) < 0.05f && isRightGrapped;
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

    public void RemoveOutline()
    {
        Outlinable[] outlinables = GetComponentsInChildren<Outlinable>();
        foreach (Outlinable outline in outlinables)
        {
            outline.enabled = false;
        }
    }

    public void TriggerLeg()
    {
        _isTrigger = true;
    }

    public void UnTriggerLeg()
    {
        _isTrigger = false;
    }
}
