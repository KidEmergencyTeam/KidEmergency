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
    
    [Header("액션")]
    public ShowOptionAction showOptionAction;
    public ChangeSceneAction changeSceneAction;
    public EarthquakeAction earthquakeAction; 
    public ChangeViewAction changeViewAction;
    public PlaceObjectAction placeObjectAction;
    public HighlightObjectAction highlightObjectAction;
    public FixingBagAction fixingBagAction;
    
    private event Action OnActionComplete; // 액션 타입을 Show Dialog 로 변경하는 이벤트

    private void Start()
    {
        currentAction = ActionType.Basic;
        
        OnActionComplete += () =>
        {
            currentAction = ActionType.ShowDialog;
            UpdateAction();
        };
    }

    public void UpdateAction() 
    {
        print($"상태 업데이트 성공! 현재 상태: {currentAction}");
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
            
            case ActionType.HighlightObject:
                if (highlightObjectAction != null)
                {
                    highlightObjectAction.StartAction();
                    StartCoroutine(WaitForActionComplete(highlightObjectAction));
                }
                break;
            
            case ActionType.FixingBag:
                if (fixingBagAction != null)
                {
                    fixingBagAction.StartAction();
                    StartCoroutine(WaitForActionComplete(fixingBagAction));
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

    private void ActionEventComplete()
    {
        OnActionComplete?.Invoke();
    }
}
