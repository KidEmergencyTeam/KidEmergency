using System;
using System.Collections.Generic;
using UnityEngine;

public class FEScene : MonoBehaviour
{
	public static FEScene Instance { get; set; }

	public List<JSWDialog> dialogueData = new List<JSWDialog>();
	public string[] currentDialog;
	public int currentDialogIndex = 0;

	private FEStateMachine stateMachine;
	private FEState currentState;

	private Grabber _leftGrabber;
	private Grabber _rightGrabber;
	private Grabbable _body;
	private Grabbable _pin;
	private Grabbable _handle;
	private Grabbable _hose;
	private Fire _fire;

	private void Start()
	{
		stateMachine = new FEStateMachine(this);
	}

	private void Update()
	{
		stateMachine.currentState.ExecuteState(this);
	}

	private void ChangeState(FEState newState)
	{
		stateMachine.ChangeState(newState);
		currentState = newState;
	}
}