using UnityEngine;

public class BowState : State
{
	public bool isBow = false;
	public float bowThreshold = 0.5f; // 숙이는 기준 높이 (단위: 미터)

	public Transform xrOrigin;
	public Transform leftFootIk;
	public Transform rightFootIk;
	public Transform leftFoot;
	public Transform rightFoot;
	public Transform body;

	public float bowYValue = 0.3f;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Bow state");
	}

	public override void Execute(PlayerController player)
	{
		// 현재 헤드셋(카메라)의 Y 좌표 가져오기
		float headHeight = Camera.main.transform.position.y;

		// 기준보다 낮아지면 숙이기
		if (headHeight < bowThreshold && !isBow)
		{
			isBow = true;
			xrOrigin.localPosition -= new Vector3(0, bowYValue, 0);
			leftFootIk.localPosition += new Vector3(0, bowYValue, 0);
			rightFootIk.localPosition += new Vector3(0, bowYValue, 0);
			leftFoot.localPosition += new Vector3(0, bowYValue, 0);
			rightFoot.localPosition += new Vector3(0, bowYValue, 0);
			body.localPosition -= new Vector3(0, bowYValue, 0);
		}
		// 일정 높이 이상 올라가면 다시 서기
		else if (headHeight > bowThreshold && isBow)
		{
			isBow = false;
			xrOrigin.localPosition += new Vector3(0, bowYValue, 0);
			leftFootIk.localPosition -= new Vector3(0, bowYValue, 0);
			rightFootIk.localPosition -= new Vector3(0, bowYValue, 0);
			leftFoot.localPosition -= new Vector3(0, bowYValue, 0);
			rightFoot.localPosition -= new Vector3(0, bowYValue, 0);
			body.localPosition += new Vector3(0, bowYValue, 0);
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}