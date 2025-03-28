using System.Collections;
using System.Linq;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OutlineObjectAction : MonoBehaviour, IActionEffect
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
        EnableAllHighlightEffects();
        
        for (int i = 0; i < dialogData.parentName.Length; i++)
        {
            GameObject outlineEffect = GameObject.Find(dialogData.parentName[i]);
            
            for (int j = 0; j < outlineEffect.transform.childCount; j++)
            {
                GameObject obj = outlineEffect.transform.GetChild(j).gameObject;
                if (obj.GetComponent<Outlinable>() != null && obj.GetComponent<BaseOutlineObject>())
                {
                    Outlinable outlinable = obj.GetComponent<Outlinable>();
                    outlinable.enabled = true;
                    BaseOutlineObject outline = obj.GetComponent<BaseOutlineObject>();
                    outline.enabled = true;
                }
            }
            
            yield return null;
        }

        _isComplete = true;
    }
    
    private void EnableAllHighlightEffects()
    {
        Outlinable[] outlinables = FindObjectsOfType<Outlinable>();
        foreach (Outlinable outline in outlinables)
        {
            outline.enabled = false;
        }
    }
} 
