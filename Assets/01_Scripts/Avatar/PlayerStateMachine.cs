using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 수행하는 행동 상태 정의
public enum PlayerState
{
	None,
	Button,
	Pick,
	Hold,
	Walk,
	Push,
	Bow
}

// 특정 시나리오별 플레이어 상태 흐름을 저장하는 데이터 클래스
public static class ModeStateData
{
	public static readonly Dictionary<string, List<PlayerState>> ModeSequences =
		new Dictionary<string, List<PlayerState>>
		{
			{
				"FireKinder",
				new List<PlayerState>
				{
					PlayerState.None, PlayerState.Button, PlayerState.Pick,
					PlayerState.Hold, PlayerState.Walk, PlayerState.Button,
					PlayerState.Bow, PlayerState.Button
				}
			},
			{
				"FireSchool",
				new List<PlayerState>
				{
					PlayerState.None, PlayerState.Button, PlayerState.Pick,
					PlayerState.Hold, PlayerState.Walk, PlayerState.Push, PlayerState.Bow,
					PlayerState.Button, PlayerState.Button
				}
			}
		};
}

// 플레이어 상태를 순서대로 관리하는 상태 머신
public class PlayerStateMachine
{
	private Queue<PlayerState> _stateQueue;
	public PlayerState CurrentState { get; private set; }

	public event Action<PlayerState> OnStateChanged;

	public PlayerStateMachine(List<PlayerState> stateSequence)
	{
		_stateQueue = new Queue<PlayerState>(stateSequence);
		CurrentState = _stateQueue.Count > 0
			? _stateQueue.Dequeue()
			: PlayerState.None;
	}

	public void MoveToNextState()
	{
		Debug.Log(4);
		if (_stateQueue.Count > 0)
		{
			Debug.Log(5);
			CurrentState = _stateQueue.Dequeue();
			Debug.Log(6);
			OnStateChanged?.Invoke(CurrentState);
			Debug.Log(7);
		}
		else
		{
			CurrentState = PlayerState.None; // 모든 상태 종료
			OnStateChanged?.Invoke(CurrentState);
		}
	}

	public bool IsFinished() => CurrentState == PlayerState.None;
}