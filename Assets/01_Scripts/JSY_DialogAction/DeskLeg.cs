using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using BitStream = Fusion.Protocol.BitStream;

public class DeskLeg : MonoBehaviour
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;
    [SerializeField] private GameObject[] _legs;

    private float _durationTime = 0f; // 지속 시간
    private float _endTime = 5f; // 종료 시간
    public bool isComplete = false;

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
        this.enabled = true;
        for (int i = 0; i < _legs.Length; i++)
        {
            bool isInteractable = Vector3.Distance(_legs[i].transform.position, _leftController.transform.position) < 0.1f &&
                                  _leftRay.hasSelection ||
                                  Vector3.Distance(_legs[i].transform.position, _rightController.transform.position) < 0.1f &&
                                  _rightRay.hasSelection;
            if (isInteractable)
            {
                UIManager.Instance.CloseWarningUI();
            
                _durationTime += Time.deltaTime;
                if (_durationTime >= _endTime)
                {
                    isComplete = true;
                }
            }
        
            else
            {
                _durationTime = 0f;
            
                UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
                UIManager.Instance.OpenWarningUI();
                isComplete = false;
            }
        }
    }

    public bool IsHoldComplete()
    {
        if (isComplete)
        {
            return true;
        }
        
        else return false;
    }
}
