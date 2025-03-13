using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    [SerializeField] private GameObject bag;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        StartCoroutine(FindBagObject());
    }

    private void Update()
    {
        if (bag != null)
        {
            StopCoroutine(FindBagObject());
        }
    }

    public void StartAction()
    {
        _isComplete = false;
        TestAction();
        _isComplete = true; // 테스트용. 기능 제대로 구현하면 이동 예정
    }

    private IEnumerator FindBagObject()
    {
        while (bag == null)
        {
            print("가방이 없음 가방 찾는 중");
            bag = GameObject.Find("Backpack");
            yield return new WaitForSeconds(2f);
        }
    }
    
    private void TestAction()
    {
        // bag.transform.position = Vector3.zero;
        print("가방 고정됨");
    }
}
