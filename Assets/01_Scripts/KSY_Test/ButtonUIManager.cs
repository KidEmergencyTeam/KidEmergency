using UnityEngine;
using System.Collections.Generic;

// 버튼 위에 레이가 있을 때 레이 전환이 이루어지면 버튼 실행을 막기 위한 스크립트
public class ButtonUIManager : DisableableSingleton<ButtonUIManager>
{
    // TestButton2.cs를 가진 버튼
    private List<TestButton2> buttonList = new List<TestButton2>();

    private void Start()
    {
        // "Button" 태그가 붙은 모든 버튼을 찾아 리스트에 추가
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject btn in buttons)
        {
            TestButton2 tb = btn.GetComponent<TestButton2>();
            if (tb != null)
            {
                buttonList.Add(tb);
            }
        }
    }

    // 버튼 입력을 비활성화
    public void DisableAllButtonInputs()
    {
        foreach (TestButton2 tb in buttonList)
        {
            tb.DisableInput();
        }
        Debug.Log("[ButtonUIManager] 버튼 입력 비활성화");
    }

    // 버튼 입력을 활성화
    public void EnableAllButtonInputs()
    {
        foreach (TestButton2 tb in buttonList)
        {
            tb.EnableInput();
        }
        Debug.Log("[ButtonUIManager] 버튼 입력 활성화");
    }
}
