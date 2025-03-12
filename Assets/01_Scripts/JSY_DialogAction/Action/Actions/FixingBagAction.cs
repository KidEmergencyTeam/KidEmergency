using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixingBagAction : MonoBehaviour, IActionEffect
{
    private bool isComplete = false;
    public bool IsActionComplete => isComplete;

    public void StartAction()
    {
        isComplete = false;
        TestAction();
        isComplete = true; // 테스트용. 기능 제대로 구현하면 이동 예정
    }


    private void TestAction()
    {
        print("가방 고정됨");
    }
}
