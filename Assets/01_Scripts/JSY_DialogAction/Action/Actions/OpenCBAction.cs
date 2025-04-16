using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenCBAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _cb;
    [SerializeField] private GameObject _circuitBox;
    [SerializeField] private GameObject _highlighter;
    [SerializeField] private ActionBasedController[] _controller;
    [SerializeField] private Collider[] _hand;
    
    private bool _isComplete = false;
    public bool isBtLeftTrigger = false;
    public bool isBtRightTrigger = false;
    
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        for (int i = 0; i < _hand.Length; i++)
        {
            _hand[i].enabled = false;
        }
    }

    public void StartAction()
    {
        _isComplete = false;
        isBtLeftTrigger = false;
        isBtRightTrigger = false;
        StartCoroutine(OpenCircuitBox());
    }

    private IEnumerator OpenCircuitBox()
    {
        _highlighter.SetActive(true);
        for (int i = 0; i < _hand.Length; i++)
        {
            _hand[i].enabled = true;
        }
        BaseOutlineObject outline = _cb.GetComponent<BaseOutlineObject>();
        outline.enabled = true;
        
        while (!_isComplete)
        {
            if (isBtLeftTrigger)
            {
                if (_controller[0].selectAction.action.ReadValue<float>() >= 1)
                {
                    _circuitBox.transform.localPosition = new Vector3(0, -0.02f, 0.15f);
                    _circuitBox.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

                    _highlighter.SetActive(false);
                    for (int i = 0; i < _hand.Length; i++)
                    {
                        _hand[i].enabled = false;
                    }
                    _isComplete = true;   
                }
            }

            if (isBtRightTrigger)
            {
                if (_controller[1].selectAction.action.ReadValue<float>() >= 1)
                {
                    _circuitBox.transform.localPosition = new Vector3(0, -0.02f, 0.15f);
                    _circuitBox.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));

                    _highlighter.SetActive(false);
                    for (int i = 0; i < _hand.Length; i++)
                    {
                        _hand[i].enabled = false;
                    }
                    _isComplete = true;   
                }
            }
            
            yield return null;
        }
    }
}
