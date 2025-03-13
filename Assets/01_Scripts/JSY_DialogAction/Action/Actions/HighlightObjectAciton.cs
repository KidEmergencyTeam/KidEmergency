using System.Collections;
using System.Linq;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
            
            if (outlineEffect.CompareTag("BaseObject"))
            {
                outlineEffect.AddComponent<BaseOutlineObject>();
            }
            else if (outlineEffect.CompareTag("GrabObject"))
            {
                outlineEffect.AddComponent<GrabOutlineObject>();
                outlineEffect.AddComponent<XRGrabInteractable>();
            }
            
            yield return null;
        }

        print($"{dialogData.objectsNames.Length}개의 오브젝트 강조됨");
        _isComplete = true;
    }
    
    private void DeleteAllHighlightEffects()
    {
        Outlinable[] outlinables = FindObjectsOfType<Outlinable>();
        foreach (Outlinable outline in outlinables)
        {
            Destroy(outline);
        }
    }
} 
