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
        if (dialogData.parentName != null)
        {
            for (int i = 0; i < dialogData.parentName.Length; i++)
            {
                GameObject obj = GameObject.Find(dialogData.parentName[i]);
                for (int j = 0; j < obj.transform.childCount; j++)
                {
                    obj.transform.GetChild(j).gameObject.SetActive(true);
                    yield return null;
                }
            }
            
            _isComplete = true;
        }
    
        else
        {
            _isComplete = false;
            print("SO에 오브젝트 이름이 없음");
        }
    }
    
}
