using UnityEngine;

public class BowState : State
{
	public bool isBow = false;
	public float bowThreshold = 1.2f; // 숙이는 기준 높이 (단위: 미터)

	public float bowYValue = 0.35f;

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
			player.xrOrigin.localPosition -= new Vector3(0, bowYValue, 0);
			player.leftFootIkTarget.localPosition += new Vector3(0, bowYValue, 0);
			player.rightFootIkTarget.localPosition += new Vector3(0, bowYValue, 0);
			player.leftFootTarget.localPosition += new Vector3(0, bowYValue, 0);
			player.rightFootTarget.localPosition += new Vector3(0, bowYValue, 0);
			player.bodyTarget.localPosition -= new Vector3(0, bowYValue, 0);
			player.playerTargetFollower.followTargets[2].posOffset +=
				new Vector3(0, bowYValue, 0);
			player.playerTargetFollower.followTargets[3].posOffset +=
				new Vector3(0, bowYValue, 0);
		}
		// 일정 높이 이상 올라가면 다시 서기
		else if (headHeight > bowThreshold - bowYValue && isBow)
		{
			isBow = false;
			player.xrOrigin.localPosition += new Vector3(0, bowYValue, 0);
			player.leftFootIkTarget.localPosition -= new Vector3(0, bowYValue, 0);
			player.rightFootIkTarget.localPosition -= new Vector3(0, bowYValue, 0);
			player.leftFootTarget.localPosition -= new Vector3(0, bowYValue, 0);
			player.rightFootTarget.localPosition -= new Vector3(0, bowYValue, 0);
			player.bodyTarget.localPosition += new Vector3(0, bowYValue, 0);
			player.playerTargetFollower.followTargets[2].posOffset -=
				new Vector3(0, bowYValue, 0);
			player.playerTargetFollower.followTargets[3].posOffset -=
				new Vector3(0, bowYValue, 0);
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}