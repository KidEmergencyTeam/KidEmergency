using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExitLine : MonoBehaviour
{
    public Highlighter highlighter;
    [SerializeField] private ActionBasedController[] _ctrl;
    [SerializeField] private XRRayInteractor[] _ray;
    public bool isSelected = false;

    public void ExitLineInteraction()
    {
        StartCoroutine(ExitLineCoroutine());
    }

    private IEnumerator ExitLineCoroutine()
    {
        highlighter.gameObject.SetActive(true);
        
        while (!isSelected)
        {
            bool isLeftSelected = _ray[0].hasHover && _ctrl[0].selectAction.action.ReadValue<float>() > 0.5f;
            bool isRightSelected = _ray[1].hasHover && _ctrl[1].selectAction.action.ReadValue<float>() > 0.5f;
            if (isLeftSelected || isRightSelected)
            {
                highlighter.gameObject.SetActive(false);
                isSelected = true;
            }
            
            yield return null;
        }
    }
}
