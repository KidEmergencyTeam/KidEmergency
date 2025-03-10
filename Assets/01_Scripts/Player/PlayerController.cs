using UnityEngine;

// 플레이어 컨트롤러 (현재 상태를 업데이트 및 완료 처리)
public class PlayerController : MonoBehaviour
{
	private PlayerState _currentState;

	private void Start()
	{
		if (ModeController.Instance?.StateMachine != null)
		{
			ModeController.Instance.StateMachine.OnStateChanged +=
				state => _currentState = state;
		}
	}

	private void Update()
	{
		ModeController.Instance.StateMachine.Execute(this);
		if (Input.GetKeyDown(KeyCode.Z))
		{
			Debug.Log("다음 상태로 넘어갑니다.");
			ModeController.Instance.StateMachine.MoveToNextState(this);
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			Debug.Log($"현재 상태: {_currentState}");
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log("모드를 FireKinder로 변경합니다.");
			ModeController.Instance.ChangeMode("FireKinder");
		}
		else if (Input.GetKeyDown(KeyCode.V))
		{
			Debug.Log("모드를 FireSchool로 변경합니다.");
			ModeController.Instance.ChangeMode("FireSchool");
		}
	}

	public void ChangeStateToNone()
	{
		_currentState = PlayerState.None;
		ModeController.Instance.StateMachine.ChangeStateToNone(this);
	}
}