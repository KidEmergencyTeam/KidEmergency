using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Serialization;
using XRController = UnityEngine.XR.Interaction.Toolkit.XRController;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    [SerializeField] private GameObject[] _bag;
    private GameObject _xrParent;
    private Vector3 _fixPos;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        // 멀티일 시 고유 ID로 먼저 찾고 Canparent 찾기
        _xrParent = GameObject.Find("CamParent");
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(TestAction());
    }

    public void StartMultiModeAction()
    {
        _isComplete = false;
    }

    private IEnumerator TestAction()
    {
        // 멀티일 시 고유한 뭔가(ex. ID)를 찾은 뒤에 GetComponentInChildren으로 진행 예정
        Vector3 originPos = FindObjectOfType<XRController>().transform.localPosition;
        originPos.y += 0.5f;
        _fixPos = originPos;

        while (!_isComplete)
        {
            for (int i = 0; i < _bag.Length; i++)
            {
                if (Vector3.Distance(_bag[i].transform.position, _fixPos) < 0.1f)
                {
                    _bag[i].transform.position = _fixPos;
                    _bag[i].transform.SetParent(_xrParent.transform);

                    Rigidbody rb = _bag[i].GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                }
            }
            
            yield return null;
        }
        
        print("가방 고정됨");
        _isComplete = true;
    }
}
