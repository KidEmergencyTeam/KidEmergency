using UnityEngine;

public class DownState : State
{
	public bool isDown = false;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Down state");
	}

	public override void Execute(PlayerController player)
	{
		float downThreshold = player.downThreshold;
		float downYValue = player.downYValue;
		float headHeight = Camera.main.transform.position.y;

		Vector3 downOffset =
			player.transform.InverseTransformDirection(Vector3.up * downYValue);

		if (headHeight < downThreshold && !isDown)
		{
			isDown = true;

			// 월드 기준으로 이동하여 회전 영향을 최소화
			player.xrOrigin.position -= Vector3.up * downYValue;

			// 발 위치 조정 (회전 보정 적용)
			player.leftFootIkTarget.position += downOffset;
			player.rightFootIkTarget.position += downOffset;
			player.leftFootTarget.position += downOffset;
			player.rightFootTarget.position += downOffset;
			player.bodyTarget.position -= downOffset;

			// 팔 IK 보정 (회전 보정 적용)
			player.playerTargetFollower.followTargets[2].posOffset += downOffset;
			player.playerTargetFollower.followTargets[3].posOffset += downOffset;
		}
		else if (headHeight > downThreshold - downYValue && isDown)
		{
			isDown = false;

			// 원래 위치로 복귀
			player.xrOrigin.position += Vector3.up * downYValue;

			player.leftFootIkTarget.position -= downOffset;
			player.rightFootIkTarget.position -= downOffset;
			player.leftFootTarget.position -= downOffset;
			player.rightFootTarget.position -= downOffset;
			player.bodyTarget.position += downOffset;

			player.playerTargetFollower.followTargets[2].posOffset -= downOffset;
			player.playerTargetFollower.followTargets[3].posOffset -= downOffset;
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Down state");
	}
}