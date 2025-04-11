using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class DeskLeg : MonoBehaviour
{
    
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private GameObject[] _legs;
    [SerializeField] private GameObject[] _hand;
    [SerializeField] private GameObject[] _legsHighlight;
    [SerializeField] private ActionBasedController[] _controller;
    
    private float _durationTime = 0f; // 지속 시간
    private float _endTime = 5f; // 종료 시간
    public bool isHoldComplete = false;
    
    private void Start()
    {
        for (int i = 0; i < _legsHighlight.Length; i++)
        {
            _legsHighlight[i].SetActive(false);
            _legs[i].GetComponent<BaseOutlineObject>().enabled = false;
        }

        this.enabled = false;
    }

    private void Update()
    {
        Holding();
    }

    public void Holding()
    {
        for (int i = 0; i < _legsHighlight.Length; i++)
        {
            _legsHighlight[i].SetActive(true);
            _legs[i].GetComponent<BaseOutlineObject>().enabled = true;
        }
        
        bool isLeftGrapped = _controller[0].selectAction.action.ReadValue<float>() >= 1;
        bool isRightGrapped = _controller[1].selectAction.action.ReadValue<float>() >= 1;
        
        bool isInteractable = Vector3.Distance(_legs[0].transform.position, _hand[0].transform.position) < 0.15f &&
                             isLeftGrapped &&
                             Vector3.Distance(_legs[1].transform.position, _hand[1].transform.position) < 0.15f &&
                             isRightGrapped;
        
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
        for (int i = 0; i < _legsHighlight.Length; i++)
        {
            _legsHighlight[i].SetActive(false);
        }
    }
}
