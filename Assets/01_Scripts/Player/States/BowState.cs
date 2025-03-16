using UnityEngine;

public class BowState : State
{
	public bool isBow = false;
	private float _bowThreshold = 1.2f; // 숙이는 기준 높이 (단위: 미터)
	private float _standThreshold = 1.3f; // 다시 서는 기준 높이

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Bow state");
	}

	public override void Execute(PlayerController player)
	{
		// 현재 헤드셋(카메라)의 Y 좌표 가져오기
		float headHeight = Camera.main.transform.position.y;

		// 기준보다 낮아지면 숙이기
		if (headHeight < _bowThreshold && !isBow)
		{
			isBow = true;
			Debug.Log("Player is bowing.");
		}
		// 일정 높이 이상 올라가면 다시 서기
		else if (headHeight > _standThreshold && isBow)
		{
			isBow = false;
			Debug.Log("Player is standing up.");
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}