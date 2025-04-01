using UnityEngine;

public class PlayerRig : MonoBehaviour
{
	public enum State
	{
		None,
		Bow,
		Down
	}

	public State currentState = State.None;

	public TargetFollower playerTargetFollower;
	public Transform xrOrigin;
	public Transform mainCameraTarget;
	public Transform leftFootIkTarget;
	public Transform rightFootIkTarget;
	public Transform leftFootTarget;
	public Transform rightFootTarget;
	public Transform bodyTarget;

	[Header("숙이는 기준 높이")] public float bowThreshold = 1.2f;
	[Header("숙일 때 낮출 카메라 높이")] public float bowYValue = 0.35f;
	[Header("엎드릴 때 낮출 카메라 높이")] public float downYValue = 0.35f;

	private bool isAction = false;

	private void Update()
	{
		switch (currentState)
		{
			case State.None:
				SetNone();
				break;
			case State.Bow:
				SetBow();
				break;
			case State.Down:
				SetDown();
				break;
		}
	}

	private void SetNone()
	{
		if (isAction)
		{
			isAction = false;
			xrOrigin.localPosition += new Vector3(0, downYValue, 0);
			mainCameraTarget.localPosition -= new Vector3(0, downYValue, 0);
			leftFootIkTarget.localPosition -= new Vector3(0, downYValue, 0);
			rightFootIkTarget.localPosition -= new Vector3(0, downYValue, 0);
			leftFootTarget.localPosition -= new Vector3(0, downYValue, 0);
			rightFootTarget.localPosition -= new Vector3(0, downYValue, 0);
			bodyTarget.localPosition += new Vector3(0, downYValue, 0);

			// Hand 위치 조정을 위함
			playerTargetFollower.followTargets[2].posOffset -=
				new Vector3(0, downYValue, 0);
			playerTargetFollower.followTargets[3].posOffset -=
				new Vector3(0, downYValue, 0);
		}
	}

	private void SetBow()
	{
		float headHeight = Camera.main.transform.position.y;

		if (headHeight < bowThreshold && !isAction)
		{
			isAction = true;
			xrOrigin.localPosition -= new Vector3(0, bowYValue, 0);
			mainCameraTarget.localPosition += new Vector3(0, bowYValue, 0);
			leftFootIkTarget.localPosition += new Vector3(0, bowYValue, 0);
			rightFootIkTarget.localPosition += new Vector3(0, bowYValue, 0);
			leftFootTarget.localPosition += new Vector3(0, bowYValue, 0);
			rightFootTarget.localPosition += new Vector3(0, bowYValue, 0);
			bodyTarget.localPosition -= new Vector3(0, bowYValue, 0);

			// Hand 위치 조정을 위함
			playerTargetFollower.followTargets[2].posOffset +=
				new Vector3(0, bowYValue, 0);
			playerTargetFollower.followTargets[3].posOffset +=
				new Vector3(0, bowYValue, 0);
		}
		// 일정 높이 이상 올라가면 다시 서기
		else if (headHeight > bowThreshold - bowYValue && isAction)
		{
			isAction = false;
			xrOrigin.localPosition += new Vector3(0, bowYValue, 0);
			mainCameraTarget.localPosition -= new Vector3(0, bowYValue, 0);
			leftFootIkTarget.localPosition -= new Vector3(0, bowYValue, 0);
			rightFootIkTarget.localPosition -= new Vector3(0, bowYValue, 0);
			leftFootTarget.localPosition -= new Vector3(0, bowYValue, 0);
			rightFootTarget.localPosition -= new Vector3(0, bowYValue, 0);
			bodyTarget.localPosition += new Vector3(0, bowYValue, 0);

			// Hand 위치 조정을 위함
			playerTargetFollower.followTargets[2].posOffset -=
				new Vector3(0, bowYValue, 0);
			playerTargetFollower.followTargets[3].posOffset -=
				new Vector3(0, bowYValue, 0);
		}
	}

	private void SetDown()
	{
		if (!isAction)
		{
			isAction = true;
			xrOrigin.localPosition -= new Vector3(0, downYValue, 0);
			mainCameraTarget.localPosition += new Vector3(0, bowYValue, 0);
			leftFootIkTarget.localPosition += new Vector3(0, downYValue, 0);
			rightFootIkTarget.localPosition += new Vector3(0, downYValue, 0);
			leftFootTarget.localPosition += new Vector3(0, downYValue, 0);
			rightFootTarget.localPosition += new Vector3(0, downYValue, 0);
			bodyTarget.localPosition -= new Vector3(0, downYValue, 0);

			// Hand 위치 조정을 위함
			playerTargetFollower.followTargets[2].posOffset +=
				new Vector3(0, downYValue, 0);
			playerTargetFollower.followTargets[3].posOffset +=
				new Vector3(0, downYValue, 0);
		}
	}
}