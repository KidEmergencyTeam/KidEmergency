using System.Collections;
using UnityEditor.AssetImporters;
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
        if (dialogData.objects != null && dialogData.objectsNames != null)
        {
            for (int i = 0; i < dialogData.objects.Length && i < dialogData.objectsNames.Length; i++)
            {
                GameObject clone = Instantiate(dialogData.objects[i]);
                clone.name = dialogData.objectsNames[i];
                yield return null;
            }
            
            _isComplete = true;
        }

        else
        {
            print("SO에 오브젝트나 오브젝트 이름이 없음");
        }
    }
    
}
