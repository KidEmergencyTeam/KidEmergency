using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
        TestAction();
        // _isComplete = true; // 테스트용. 기능 제대로 구현하면 이동 예정
    }


    private void TestAction()
    {
        print("가방 고정됨");
    }
}
