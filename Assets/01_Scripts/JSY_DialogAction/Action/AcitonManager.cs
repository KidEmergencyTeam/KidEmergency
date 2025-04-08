using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ActionManager : SingletonManager<ActionManager>
{
	public ActionType currentAction;
	public DialogData beforeDialog;
	public DialogData currentDialog;

	[Header("액션")] public ShowOptionAction showOptionAction;
	public ChangeSceneAction changeSceneAction;
	public EarthquakeAction earthquakeAction;
	public ChangeViewAction changeViewAction;
	public PlaceObjectAction placeObjectAction;
	public FixBagAction fixBagAction;
	public HoldLegAction holdLegAction;
	public CloseGVAction closeGVAction;
	public OpenCBAction openCBAction;
	public LowerCLAction lowerCLAction;
	public SelectLineAction selectLineAction;
	public EndGameAction endGameAction;

	public AudioSource actionAudio;
	private event Action OnActionComplete; // 액션 타입을 Show Dialog 로 변경하는 이벤트
	

	private void Start()
	{
		StartCoroutine(StartDialog());
		currentAction = ActionType.Basic;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
		OnActionComplete += () =>
		{
			currentAction = ActionType.ShowDialog;
			UpdateAction();
		};
	}

	public void UpdateAction()
	{
		switch (currentAction)
		{
			case ActionType.Basic:
				break;

			case ActionType.ShowDialog:
				if (currentDialog.dialogs != null)
				{
					StartCoroutine(DialogManager.Instance.ShowDialog());
				}
				else
				{
					print("현재 SO에 저장된 다이얼로그가 없음~");
				}

				break;

			case ActionType.ShowOption:
				if (showOptionAction != null)
				{
					showOptionAction.StartAction();
					StartCoroutine(WaitForActionComplete(showOptionAction));
				}

				break;

			case ActionType.ChangeScene:
				if (changeSceneAction != null)
				{
					changeSceneAction.StartAction();
					StartCoroutine(WaitForActionComplete(changeSceneAction));
				}

				break;

			case ActionType.Earthquake:
				if (earthquakeAction != null)
				{
					earthquakeAction.StartAction();
					StartCoroutine(WaitForActionComplete(earthquakeAction));
				}

				break;

			case ActionType.ChangeView:
				if (changeViewAction != null)
				{
					changeViewAction.StartAction();
					StartCoroutine(WaitForActionComplete(changeViewAction));
				}

				break;

			case ActionType.PlaceObject:
				if (placeObjectAction != null)
				{
					placeObjectAction.StartAction();
					StartCoroutine(WaitForActionComplete(placeObjectAction));
				}

				break;

			case ActionType.FixBag:
				if (fixBagAction != null)
				{
					fixBagAction.StartAction();
					StartCoroutine(WaitForActionComplete(fixBagAction));
				}

				break;
			
			case ActionType.HoldLeg:
				if (holdLegAction != null)
				{
					holdLegAction.StartAction();
					StartCoroutine(WaitForActionComplete(holdLegAction));
				}

				break;
			
			case ActionType.CloseGasValve:
				if (closeGVAction != null)
				{
					closeGVAction.StartAction();
					StartCoroutine(WaitForActionComplete(closeGVAction));
				}
				break;
			
			case ActionType.OpenCircuitBox:
				if (openCBAction != null)
				{
					openCBAction.StartAction();
					StartCoroutine(WaitForActionComplete(openCBAction));
				}
				break;
			
			case ActionType.LowerCircuitLever:
				if (lowerCLAction != null)
				{
					lowerCLAction.StartAction();
					StartCoroutine(WaitForActionComplete(lowerCLAction));
				}
				break;
			
			case ActionType.SelectGuideLine:
				if (selectLineAction != null)
				{
					selectLineAction.StartAction();
					StartCoroutine(WaitForActionComplete(selectLineAction));
				}
				break;
			
			case ActionType.EndGame:
				if (endGameAction != null)
				{
					endGameAction.StartAction();
				}
				
				break;
		}
	}

	private IEnumerator WaitForActionComplete(IActionEffect effect)
	{
		yield return new WaitUntil(() => effect.IsActionComplete);
		if (currentDialog.dialogs != null)
		{
			Invoke("ActionEventComplete", 1f);
		}
		else
		{
			print("현재 다이얼로그에 대사가 없음");
		}
	}

	private IEnumerator StartDialog()
	{
		yield return StartCoroutine(OVRScreenFade.Instance.Fade(1f, 0f));
		DialogManager.Instance.DialogStart();
	}

	private void ActionEventComplete()
	{
		OnActionComplete?.Invoke();
	}
}