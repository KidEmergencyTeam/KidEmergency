using System;
using System.Collections.Generic;

// 플레이어 상태를 순서대로 관리하는 상태 머신
public class PlayerStateMachine
{
	private State _currentState;

	private Queue<PlayerState> _stateQueue;
	public PlayerState CurrentState { get; private set; }

	public event Action<PlayerState> OnStateChanged;

	public PlayerStateMachine(List<PlayerState> stateSequence)
	{
		_stateQueue = new Queue<PlayerState>(stateSequence);
		CurrentState = _stateQueue.Count > 0
			? _stateQueue.Dequeue()
			: PlayerState.None;
		Setup(CurrentState);
	}

	public void MoveToNextState(PlayerController player)
	{
		_currentState.Exit(player);
		if (_stateQueue.Count > 0)
		{
			CurrentState = _stateQueue.Dequeue();
			OnStateChanged?.Invoke(CurrentState);
		}
		else
		{
			CurrentState = PlayerState.None; // 모든 상태 종료
			OnStateChanged?.Invoke(CurrentState);
		}

		Setup(CurrentState);
	}

	public void ChangeStateToNone(PlayerController player)
	{
		CurrentState = PlayerState.None;
		_currentState.Exit(player);
		Setup(CurrentState);
	}

	public void Execute(PlayerController player)
	{
		if (CurrentState != PlayerState.None)
		{
			_currentState.Execute(player);
		}
	}

	private void Setup(PlayerState state)
	{
		switch (state)
		{
			case PlayerState.None:
				_currentState = new NoneState();
				break;
			case PlayerState.Button:
				_currentState = new ButtonState();
				break;
			case PlayerState.Pick:
				_currentState = new PickState();
				break;
			case PlayerState.Hold:
				_currentState = new HoldState();
				break;
			case PlayerState.Walk:
				_currentState = new WalkState();
				break;
			case PlayerState.Push:
				_currentState = new PushState();
				break;
			case PlayerState.Bow:
				_currentState = new BowState();
				break;
		}
	}
}