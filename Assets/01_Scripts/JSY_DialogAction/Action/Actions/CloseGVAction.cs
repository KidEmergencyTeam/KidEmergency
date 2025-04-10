using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseGVAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _target; 
    [SerializeField] private GameObject[] _hand; // 0 왼, 1 오
    [SerializeField] private GameObject _highlighter;
    [SerializeField] private float _limitRot = 90f;
    [SerializeField] private XRKnob _knob;
    
    private bool _isComplete = false;

    public bool IsActionComplete => _isComplete;
    
    private void Start()
    {
        _knob.enabled = false;
    }

    private void Update()
    {
        if (!_isComplete)
        {
            _highlighter.SetActive(true);
            _target.GetComponent<BaseOutlineObject>().enabled = true;

                _knob.enabled = true;
                print($"knob enable {_knob.enabled} ~");
                if (_knob.value >= 0.99f)
                {
                    _knob.enabled = false;
                    _knob.value = 1f;
                    _target.GetComponent<BaseOutlineObject>().enabled = false;
                    _highlighter.SetActive(false);
        
                    _isComplete = true;
                }
            
        }
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(TryCloseGV());
    }

    private IEnumerator TryCloseGV()
    {
        _highlighter.SetActive(true);
        _target.GetComponent<BaseOutlineObject>().enabled = true;
        
        while (!_isComplete)
        {
            _knob.enabled = true;
            if(_knob.value >= 0.99f)
            { 
                _knob.enabled = false;
                _knob.value = 1f;
                _target.GetComponent<BaseOutlineObject>().enabled = false;
                _highlighter.SetActive(false);
                        
                _isComplete = true;
            } 
            
            yield return null; 
        }
    }
}
