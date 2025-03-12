using System.Collections;
using System.Linq;
using EPOOutline;
using UnityEngine;

public class HighlightObjectAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SetHighlightEffect(ActionManager.Instance.beforeDialog));
    }

    private IEnumerator SetHighlightEffect(DialogData dialogData)
    {
        DeleteAllHighlightEffects();
        
        for (int i = 0; i < dialogData.objectsNames.Length; i++)
        {
            GameObject outlineEffect = GameObject.Find(dialogData.objectsNames[i]);
            Outlinable outlinable = outlineEffect.AddComponent<Outlinable>(); 
            outlinable.AddAllChildRenderersToRenderingList();
            outlineEffect.AddComponent<OutlineObjectController>();
            
            yield return null;
        }

        print($"{dialogData.objectsNames.Length}개의 오브젝트 강조됨");
        _isComplete = true;
    }

    // if (dialogData.objectsNames != null)
    // {
    //     for (int i = 0; i < dialogData.objectsNames.Length; i++)
    //     {
    //         GameObject outlineEffect = GameObject.Find(dialogData.objectsNames[i]);
    //         outlineEffect.AddComponent<Outlinable>();
    //     }
    //     print($"{dialogData.objectsNames.Length}개의 오브젝트 강조됨");
    // }
    private void DeleteAllHighlightEffects()
    {
        Outlinable[] outlinables = FindObjectsOfType<Outlinable>();
        foreach (Outlinable outline in outlinables)
        {
            Destroy(outline);
        }
    }
} 
