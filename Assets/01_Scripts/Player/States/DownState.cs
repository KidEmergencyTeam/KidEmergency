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

		// 현재 헤드셋(카메라)의 Y 좌표 가져오기
		float headHeight = Camera.main.transform.position.y;

		// 기준보다 낮아지면 숙이기
		if (headHeight < downThreshold && !isDown)
		{
			isDown = true;
			player.xrOrigin.localPosition -= new Vector3(0, downYValue, 0);
			player.leftFootIkTarget.localPosition += new Vector3(0, downYValue, 0);
			player.rightFootIkTarget.localPosition += new Vector3(0, downYValue, 0);
			player.leftFootTarget.localPosition += new Vector3(0, downYValue, 0);
			player.rightFootTarget.localPosition += new Vector3(0, downYValue, 0);
			player.bodyTarget.localPosition -= new Vector3(0, downYValue, 0);
			player.playerTargetFollower.followTargets[2].posOffset +=
				new Vector3(0, downYValue, 0);
			player.playerTargetFollower.followTargets[3].posOffset +=
				new Vector3(0, downYValue, 0);
		}
		// 일정 높이 이상 올라가면 다시 서기
		else if (headHeight > downThreshold - downYValue && isDown)
		{
			isDown = false;
			player.xrOrigin.localPosition += new Vector3(0, downYValue, 0);
			player.leftFootIkTarget.localPosition -= new Vector3(0, downYValue, 0);
			player.rightFootIkTarget.localPosition -= new Vector3(0, downYValue, 0);
			player.leftFootTarget.localPosition -= new Vector3(0, downYValue, 0);
			player.rightFootTarget.localPosition -= new Vector3(0, downYValue, 0);
			player.bodyTarget.localPosition += new Vector3(0, downYValue, 0);
			player.playerTargetFollower.followTargets[2].posOffset -=
				new Vector3(0, downYValue, 0);
			player.playerTargetFollower.followTargets[3].posOffset -=
				new Vector3(0, downYValue, 0);
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Down state");
	}
}