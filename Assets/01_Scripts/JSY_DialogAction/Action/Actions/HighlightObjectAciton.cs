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
        _isComplete = true;
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
