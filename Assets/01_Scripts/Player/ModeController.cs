using System.Collections.Generic;
using UnityEngine;

// 스테이지별로 상태 머신을 관리하는 매니저
public class ModeController : SingletonManager<ModeController>
{
	public PlayerStateMachine StateMachine { get; private set; }

	[SerializeField] private string _modeKey;

	private void Start()
	{
		ChangeMode(_modeKey);
	}

	private void HandleStateChanged(PlayerState newState)
	{
		Debug.Log($"[ModeController] 상태 변경: {newState}");
		if (newState == PlayerState.None)
		{
			Debug.Log("현재 상태가 None 입니다.");
		}
	}

	public void ChangeMode(string modeKey)
	{
		_modeKey = modeKey;
		if (ModeStateData.ModeSequences.TryGetValue(_modeKey,
			    out List<PlayerState> sequence))
		{
			StateMachine = new PlayerStateMachine(sequence);
			StateMachine.OnStateChanged += HandleStateChanged;

			Debug.Log($"ModeKey: {_modeKey}");
			Debug.Log($"Sequence:");
			foreach (var seq in sequence)
			{
				Debug.Log(seq.ToString());
			}
		}
		else
		{
			Debug.LogError($"모드 키 '{_modeKey}'가 존재하지 않습니다.");
		}
	}
}