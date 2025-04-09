using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    public InputActionProperty leftGrip;

    public InputActionProperty rightGrip;

    public Animator animator;
    
    public bool isLeftGrabbed = false;
    public bool isRightGrabbed = false;

    private RayController ray;

    private void Start()
    {
        ray = FindObjectOfType<RayController>();
    }

    void Update()
    {
        if (ray.UIActive())
        {
            animator.SetFloat("Left Trigger", 1f);
            animator.SetFloat("Right Trigger", 1f);
            animator.SetFloat("Left Grip", 0);
            animator.SetFloat("Right Grip", 0);
        }
            
        else
        {
            if (ActionManager.Instance != null)
            {
                if (ActionManager.Instance.currentAction == ActionType.OpenCircuitBox ||
                    ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
                {
                    animator.SetFloat("Left Trigger", 1f);
                    animator.SetFloat("Right Trigger", 1f);
                    animator.SetFloat("Left Grip", 0);
                    animator.SetFloat("Right Grip", 0);
                }
                
                else
                {
                    animator.SetFloat("Left Trigger", 0);
                    animator.SetFloat("Right Trigger", 0);
            
                    if (!isLeftGrabbed)
                    {
                        float leftGripValue = leftGrip.action.ReadValue<float>();
                        animator.SetFloat("Left Grip", leftGripValue);  
                    }

                    if (!isRightGrabbed)
                    {
                        float rightGripValue = rightGrip.action.ReadValue<float>();
                        animator.SetFloat("Right Grip", rightGripValue);   
                    }
                }
            }
            
            else
            {
                animator.SetFloat("Left Trigger", 0);
                animator.SetFloat("Right Trigger", 0);
            
                if (!isLeftGrabbed)
                {
                    float leftGripValue = leftGrip.action.ReadValue<float>();
                    animator.SetFloat("Left Grip", leftGripValue);  
                }

                if (!isRightGrabbed)
                {
                    float rightGripValue = rightGrip.action.ReadValue<float>();
                    animator.SetFloat("Right Grip", rightGripValue);   
                }
            }
        }
    }
}
