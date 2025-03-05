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
public static class StageStateData
{
	public static readonly Dictionary<string, List<PlayerState>> StageSequences =
		new Dictionary<string, List<PlayerState>>
		{
			{
				"FireKinder",
				new List<PlayerState>
				{
					PlayerState.Button, PlayerState.Pick, PlayerState.Hold,
					PlayerState.Walk, PlayerState.Button, PlayerState.Bow,
					PlayerState.Button
				}
			},
			{
				"FireSchool",
				new List<PlayerState>
				{
					PlayerState.Button, PlayerState.Pick, PlayerState.Hold,
					PlayerState.Walk, PlayerState.Push, PlayerState.Bow,
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

	public PlayerStateMachine(List<PlayerState> stateSequence)
	{
		_stateQueue = new Queue<PlayerState>(stateSequence);
		CurrentState = _stateQueue.Count > 0
			? _stateQueue.Dequeue()
			: PlayerState.None;
	}

	public void MoveToNextState()
	{
		if (_stateQueue.Count > 0)
		{
			CurrentState = _stateQueue.Dequeue();
		}
		else
		{
			CurrentState = PlayerState.None; // 모든 상태 종료
		}
	}

	public bool IsFinished() => CurrentState == PlayerState.None;
}

// 스테이지별로 상태 머신을 관리하는 매니저
public class StageManager : MonoBehaviour
{
	public static PlayerStateMachine CurrentStage { get; private set; }

	[SerializeField] private string stageKey; // Inspector에서 스테이지 키 설정

	private void Start()
	{
		if (StageStateData.StageSequences.TryGetValue(stageKey,
			    out List<PlayerState> sequence))
		{
			CurrentStage = new PlayerStateMachine(sequence);
		}
		else
		{
			Debug.LogError($"스테이지 키 '{stageKey}'가 존재하지 않습니다.");
		}
	}

	public static void MoveToNextState()
	{
		CurrentStage?.MoveToNextState();
		Debug.Log("다음 상태: " + CurrentStage?.CurrentState);
	}
}

// 플레이어 컨트롤러 (현재 상태를 업데이트 및 완료 처리)
public class PlayerController : MonoBehaviour
{
	private PlayerState _currentState;

	private void Update()
	{
		if (StageManager.CurrentStage != null)
		{
			_currentState = StageManager.CurrentStage.CurrentState;
		}
	}

	public void CompleteCurrentAction()
	{
		if (StageManager.CurrentStage != null &&
		    !StageManager.CurrentStage.IsFinished())
		{
			StageManager.MoveToNextState();
		}
	}
}