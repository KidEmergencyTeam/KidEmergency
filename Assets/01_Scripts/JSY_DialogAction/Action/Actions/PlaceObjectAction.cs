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
                obj.transform.gameObject.SetActive(true);
                yield return null;
            }

            if (obj.activeSelf)
            {
                _isComplete = true;
            }
        }
    
        else
        {
            _isComplete = false;
            print("SO에 오브젝트 이름이 없음");
        }
    }
    
}
