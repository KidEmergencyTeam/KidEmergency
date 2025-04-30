using UnityEngine;

public class CircuitTrigger : MonoBehaviour
{
    [SerializeField] private bool _isLever; // true면 레버, false먄 버튼

    public bool isTriggered = false;
    public OpenCBAction box;
    public LowerCLAction lever;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            if (ActionManager.Instance != null)
            {
                if (!_isLever && ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
                {
                    box.isBtLeftTrigger = true;
                    isTriggered = true;
                }
                
                else if (_isLever && ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    lever.isLvLeftTrigger = true;
                    isTriggered = true;
                }

                else return;
            }
        }
        
        if (other.CompareTag("Right Hand"))
        {
            if (ActionManager.Instance != null)
            {
                if (!_isLever && ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
                {
                    box.isBtRightTrigger = true;
                    isTriggered = true;
                }
                
                else if (_isLever && ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    lever.isLvRightTrigger = true;
                    isTriggered = true;
                }

                else return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            if (ActionManager.Instance != null)
            {
                if (!_isLever && ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
                {
                    box.isBtLeftTrigger = false;
                    isTriggered = false;
                }
                
                else if (_isLever && ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    lever.isLvLeftTrigger = false;
                    isTriggered = false;
                }

                else return;
            }
        }
        
        if (other.CompareTag("Right Hand"))
        {
            if (ActionManager.Instance != null)
            {
                if (!_isLever && ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
                {
                    box.isBtRightTrigger = false;
                    isTriggered = false;
                }
                
                else if (_isLever && ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    lever.isLvRightTrigger = false;
                    isTriggered = false;
                }

                else return;
            }
        }
    }
}