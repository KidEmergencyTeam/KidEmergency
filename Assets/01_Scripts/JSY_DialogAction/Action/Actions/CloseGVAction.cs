using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseGVAction : MonoBehaviour, IActionEffect
{
    public GameObject target; // 가스 밸브

    private XRGrabInteractable _grab;
    private float _startAngle;
    private float _currentAngle;
    private float _limitAngle = 90f;

    [SerializeField] private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;

    private void Awake()
    {
        _grab = GetComponentInChildren<XRGrabInteractable>();
    }

    private void Start()
    {
        _startAngle = target.transform.rotation.z;
        StartCoroutine(TryCloseGV());
    }

    private void Update()
    {
        print(_isComplete);
        if (target != null && _isComplete)
        {
            target.transform.rotation = Quaternion.Euler(0,0,_limitAngle);
        }
    }

    public void StartAction()
    {
        StartCoroutine(TryCloseGV());
    }

    private IEnumerator TryCloseGV()
    {
        while (!_isComplete)
        {
            print("코루틴 시작");
            _grab.enabled = true;
            _currentAngle = target.transform.rotation.z;
            float clampedAngle = Mathf.Clamp(_currentAngle, _startAngle, _startAngle + 90);

            target.transform.rotation = Quaternion.Euler(0, 0, clampedAngle);

            if (_currentAngle >= _limitAngle)
            {
                _isComplete = true;
                _grab.enabled = false;
            }
            
            yield return null;
        }
    }

}
