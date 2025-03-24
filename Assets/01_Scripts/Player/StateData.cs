using System.Collections.Generic;

// 플레이어가 수행하는 행동 상태 정의
public enum PlayerState
{
	None,
	Button,
	Pick,
	Hold,
	Push,
	Bow,
	Down,
	Up
}

// 특정 시나리오별 플레이어 상태 흐름을 저장하는 데이터 클래스
public static class ModeStateData
{
	public static readonly Dictionary<string, List<PlayerState>> ModeSequences =
		new Dictionary<string, List<PlayerState>>
		{
			{
				"Test", new List<PlayerState>
				{
					PlayerState.Bow, PlayerState.Button
				}
			},
			{
				"FireKinder", new List<PlayerState>
				{
					PlayerState.None, PlayerState.Button, PlayerState.Pick,
					PlayerState.Hold, PlayerState.Button, PlayerState.Bow,
					PlayerState.Button
				}
			},
			{
				"FireSchool", new List<PlayerState>
				{
					PlayerState.None, PlayerState.Button, PlayerState.Pick,
					PlayerState.Hold, PlayerState.Push, PlayerState.Bow,
					PlayerState.Button, PlayerState.Button
				}
			},
			{
				"EarthKinder", new List<PlayerState>
				{
					PlayerState.None, PlayerState.Button, PlayerState.Down,
					PlayerState.Up, PlayerState.Pick, PlayerState.Hold
				}
			}
		};
}