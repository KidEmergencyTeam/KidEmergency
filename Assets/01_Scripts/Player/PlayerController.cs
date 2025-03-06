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
		if (Input.GetKeyDown(KeyCode.Z))
		{
			Debug.Log("다음 상태로 넘어갑니다.");
			CompleteCurrentAction();
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			Debug.Log("모드를 FireKinder로 변경합니다.");
			ModeController.Instance.ChangeMode("FireKinder");
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log("모드를 FireSchool로 변경합니다.");
			ModeController.Instance.ChangeMode("FireSchool");
		}
	}

	public void CompleteCurrentAction()
	{
		if (ModeController.Instance?.StateMachine != null &&
		    ModeController.Instance.StateMachine.IsFinished())
		{
			_currentState = PlayerState.None;
			ModeController.Instance.StateMachine.MoveToNextState();
		}
	}

	public void ChangeStateToNone()
	{
		_currentState = PlayerState.None;
	}
}