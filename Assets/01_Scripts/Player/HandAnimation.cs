using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;

    public InputActionProperty rightPinch;
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
        //RayController ray = FindObjectOfType<RayController>();

        if (!isLeftGrabbed)
        {
            if (ray.UIActive())
            {
                animator.SetFloat("Left Trigger", 1);
            }
            
            else
            {
                float leftGripValue = leftGrip.action.ReadValue<float>();
                animator.SetFloat("Left Grip", leftGripValue);   
            }
            
        }

        if (!isRightGrabbed)
        {
            if (ray.UIActive())
            {
                animator.SetFloat("Right Trigger", 1);
            }
            else
            {
                float rightGripValue = rightGrip.action.ReadValue<float>();
                animator.SetFloat("Right Grip", rightGripValue);
                
            }
        }
    }
}
