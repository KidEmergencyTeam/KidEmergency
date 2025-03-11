using UnityEngine;

public class BowState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Bow state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Bow state...");
		// 머리 높이에 따라 일어섰는 지, 숙였는 지 bool 값 변경 및 리깅 설정
		// 일어섰을 때는 추가로 경고
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}