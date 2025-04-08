using System.Collections;
using UnityEngine;

public class PlaceObjectAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SetObjects(ActionManager.Instance.beforeDialog));
    }

    private IEnumerator SetObjects(DialogData dialogData)
    {
        if (dialogData.objectName != null)
        {
            GameObject obj = GameObject.Find(dialogData.objectName);
            if (obj != null)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    obj.transform.GetChild(i).gameObject.SetActive(true);
                    _isComplete = true;
                }
                yield return null;
            }
        }
    
        else
        {
            _isComplete = false;
            print("SO에 오브젝트 이름이 없음");
        }
    }
    
}
