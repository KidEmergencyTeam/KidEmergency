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
        
        for (int i = 0; i < dialogData.parentName.Length; i++)
        {
            GameObject outlineEffect = GameObject.Find(dialogData.parentName[i]);
            
            for (int j = 0; j < outlineEffect.transform.childCount; j++)
            {
                GameObject obj = outlineEffect.transform.GetChild(j).gameObject;
                if (obj.GetComponent<Outlinable>() == null)
                {
                    Outlinable outlinable = obj.AddComponent<Outlinable>();
                    outlinable.AddAllChildRenderersToRenderingList();
                }
                
                if (obj.CompareTag("BaseObject"))
                {
                    obj.AddComponent<BaseOutlineObject>();
                }
            }
            
            yield return null;
        }

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
