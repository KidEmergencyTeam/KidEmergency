using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExitLine : MonoBehaviour
{
    public Highlighter highlighter;
    private XRGrabInteractable _grab;
    [SerializeField] private ActionBasedController[] _ctrl;
    [SerializeField] private XRRayInteractor[] _ray;
    public bool isSelected = false;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        _grab.enabled = false;
    }

    public void ExitLineInteraction()
    {
        StartCoroutine(ExitLineCoroutine());
    }

    private IEnumerator ExitLineCoroutine()
    {
        highlighter.gameObject.SetActive(true);
        _grab.enabled = true;
        
        while (!isSelected)
        {
            bool isLeftSelected = _ray[0].hasHover && _ctrl[0].selectAction.action.ReadValue<float>() > 0.5f;
            bool isRightSelected = _ray[1].hasHover && _ctrl[1].selectAction.action.ReadValue<float>() > 0.5f;
            if (isLeftSelected || isRightSelected)
            {
                highlighter.gameObject.SetActive(false);
                _grab.enabled = false;
                isSelected = true;
            }
            
            yield return null;
        }
    }
}
