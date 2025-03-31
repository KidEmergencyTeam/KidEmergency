using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseGVAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _target; // 가스 밸브
    [SerializeField] private float _limitRot = 90f; // 목표 회전값

    private XRGrabInteractable _grabInteractable;
    private Vector3 _originPos;
    private bool _isComplete = false;
    
    public bool IsActionComplete => _isComplete;

    private void Awake()
    {
        _grabInteractable = _target.GetComponent<XRGrabInteractable>();
        _originPos = _target.transform.position;
    }

    private void Start()
    {
        _grabInteractable.enabled = false;
    }

    public void StartAction()
    {
        _isComplete = false;
        _grabInteractable.enabled = true;
        StartCoroutine(TryCloseGV());
    }

    private IEnumerator TryCloseGV()
    {
        while (!_isComplete)
        {
            _target.transform.position = _originPos;
            
            float currentZRotation = _target.transform.rotation.eulerAngles.z;
            
            if (currentZRotation >= _limitRot)
            {
                _isComplete = true;
                _grabInteractable.enabled = false;
                
                _target.transform.rotation = Quaternion.Euler(0, 0, _limitRot);
            }
            
            yield return null;
        }
    }
}
