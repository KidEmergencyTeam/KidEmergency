using System.Collections;
using UnityEngine;

public class HighlightObjectAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        SetHighlightEffect(ActionManager.Instance.currentDialog);
        _isComplete = true; // 테스트용. 기능 제대로 구현하면 이동 예정
    }

    private void SetHighlightEffect(DialogData dialogData)
    {
        DeleteAllHighlightEffects();
        
        if (dialogData.objects != null)
        {
            foreach (GameObject obj in dialogData.objects)
            {
                if (obj != null)
                {
                    print("아무튼 오브젝트 강조 됨");
                }
            }
        }
    }

    private void DeleteAllHighlightEffects()
    {
        // GameObject[] outlineObj = 게임오브젝ㅌ.파인드오브젝트오브ㅏ입<아웃라인에셋>
        // foreach(GameObjcet obj in GameObject outlineObj)
        // Destroy(obj) ?
    }
}
