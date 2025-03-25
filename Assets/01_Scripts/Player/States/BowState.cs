using UnityEngine;

public class BowState : State
{
	public bool isBow = false;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Bow state");
	}

	public override void Execute(PlayerController player)
	{
		float bowThreshold = player.bowThreshold;
		float bowYValue = player.bowYValue;
		float headHeight = Camera.main.transform.position.y;

		Vector3 bowOffset =
			player.transform.InverseTransformDirection(Vector3.up * bowYValue);

		if (headHeight < bowThreshold && !isBow)
		{
			isBow = true;

			// 월드 기준으로 이동하여 회전 영향을 최소화
			player.xrOrigin.position -= Vector3.up * bowYValue;

			// 발 위치 조정 (회전 보정 적용)
			player.leftFootIkTarget.position += bowOffset;
			player.rightFootIkTarget.position += bowOffset;
			player.leftFootTarget.position += bowOffset;
			player.rightFootTarget.position += bowOffset;
			player.bodyTarget.position -= bowOffset;

			// 팔 IK 보정 (회전 보정 적용)
			player.playerTargetFollower.followTargets[2].posOffset += bowOffset;
			player.playerTargetFollower.followTargets[3].posOffset += bowOffset;
		}
		else if (headHeight > bowThreshold - bowYValue && isBow)
		{
			isBow = false;

			// 원래 위치로 복귀
			player.xrOrigin.position += Vector3.up * bowYValue;

			player.leftFootIkTarget.position -= bowOffset;
			player.rightFootIkTarget.position -= bowOffset;
			player.leftFootTarget.position -= bowOffset;
			player.rightFootTarget.position -= bowOffset;
			player.bodyTarget.position += bowOffset;

			player.playerTargetFollower.followTargets[2].posOffset -= bowOffset;
			player.playerTargetFollower.followTargets[3].posOffset -= bowOffset;
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}