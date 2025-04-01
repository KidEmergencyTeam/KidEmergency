using System.Collections.Generic;
using UnityEngine;

public enum FEStateType
{
	FEDialog,
	FEBody,
	FEPin,
	FEHandle,
	FEHose,
	FEFire,
	FEExtinguishing,
	FEEnd
}

public class FEScene : MonoBehaviour
{
	public static FEScene Instance { get; set; }

	public List<JSWDialog> dialogueData = new List<JSWDialog>();
	public JSWDialog currentDialogData;
	public string[] currentDialog;
	public int currentDialogIndex = 0;

	private FEStateMachine _stateMachine;
	private FEStateType _startState = FEStateType.FEDialog;

	public Dictionary<FEStateType, FEState> states;

	public FEState currentState;
	public Grabbable body;
	public Grabbable pin;
	public Grabbable handle;
	public Grabbable hose;
	public Fire fire;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	private void Start()
	{
		currentDialogData = dialogueData[currentDialogIndex];
		currentDialog = dialogueData[currentDialogIndex].dialogs;
		_stateMachine = new FEStateMachine(this);
		states = new Dictionary<FEStateType, FEState>
		{
			{ FEStateType.FEDialog, new FEDialogState() },
			{ FEStateType.FEBody, new FEBodyState() },
			{ FEStateType.FEPin, new FEPinState() },
			{ FEStateType.FEHandle, new FEHandleState() },
			{ FEStateType.FEHose, new FEHoseState() },
			{ FEStateType.FEFire, new FEFireStartState() },
			{ FEStateType.FEExtinguishing, new FEExtinguishingState() },
			{ FEStateType.FEEnd, new FEEndState() }
		};
		ChangeState(_startState);
		body.isGrabbable = false;
		pin.isGrabbable = false;
		handle.isGrabbable = false;
		hose.isGrabbable = false;
		fire.gameObject.SetActive(false);
	}

	private void Update()
	{
		_stateMachine.currentState.ExecuteState(this);
	}

	public void ChangeState(FEStateType newStateType)
	{
		states.TryGetValue(newStateType, out FEState newState);
		_stateMachine.ChangeState(newState);
		currentState = newState;
	}
}