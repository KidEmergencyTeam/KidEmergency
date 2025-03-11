using System.Collections.Generic;
using UnityEngine;

public class FireKinderTest : MonoBehaviour
{
	public List<PlayerController> Players { get; private set; }

	private void Update()
	{
		foreach (PlayerController player in Players)
		{
			if (!player.isBowing)
			{
				return;
			}
		}

		// 전부 굽힌 경우 모든 플레이어의 state none으로 변경
		Debug.Log("모두 굽혔습니다.");
		foreach (PlayerController player in Players)
		{
			player.ChangeStateToNone();
		}
	}
}