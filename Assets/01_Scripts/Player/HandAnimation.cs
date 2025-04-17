using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
	public InputActionProperty leftGrip;

	public InputActionProperty rightGrip;

	public Animator animator;

	public bool isLeftGrabbed = false;
	public bool isRightGrabbed = false;

	// 경고벨 트리거 여부
	public bool isEmergencyBellTrigger = false;

    private RayController _ray;
	private Grabber _grabber;

	private void Start()
	{
		_ray = FindObjectOfType<RayController>();
		_grabber = FindObjectOfType<Grabber>();
	}

    // 우측 핸드 트리거 애니메이션 상태로 고정
    public void LockTriggerAnimation()
    {
        // 여기서는 우측 트리거를 1로, 그립은 0으로 고정
        animator.SetFloat("Right Trigger", 1f);
        animator.SetFloat("Right Grip", 0f);
    }

    protected virtual void Update()
	{
        // 비상벨 트리거가 활성화된 경우, 애니메이션을 고정시킵니다.
        if (isEmergencyBellTrigger)
        {
            LockTriggerAnimation();
            return;
        }

        if (_ray.UIActive())
		{
			if (_grabber.isOnGrabCalled)
			{
				animator.SetFloat("Right Trigger", 1f);
				animator.SetFloat("Left Grip", 1);
				animator.SetFloat("Right Grip", 0);
			}
			else
			{
				animator.SetFloat("Left Trigger", 1f);
				animator.SetFloat("Right Trigger", 1f);
				animator.SetFloat("Left Grip", 0);
				animator.SetFloat("Right Grip", 0);
			}
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
					
					if (_grabber.isOnGrabCalled)
					{
						animator.SetFloat("Left Grip", 1);
						animator.SetFloat("Right Trigger", 0);
						
						if (!isRightGrabbed)
						{
							float rightGripValue = rightGrip.action.ReadValue<float>();
							animator.SetFloat("Right Grip", rightGripValue);
						}
					}
					
					else
					{
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