using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionManager : SingletonManager<ActionManager>
{    
    public ActionType currentAction;
    public DialogData beforeDialog;
    public DialogData currentDialog;
    
    [Header("액션")]
    public EarthquakeAction earthquakeAction;
    public UnderTheDeskAction underTheDeskAction;
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
                SetOption();
                if (currentDialog.nextDialog != null)
                {
                    OnActionComplete?.Invoke();
                }
                break;
            
            case ActionType.ChangeScene:
                ChangeScene();
                Invoke("ActionCompleted", 2f);
                break;
            
            case ActionType.Earthquake:
                if (earthquakeAction != null)
                {
                    earthquakeAction.StartAction();
                    StartCoroutine(WaitForActionComplete(earthquakeAction));
                }
                break;
            
            case ActionType.UnderTheDesk:
                if (underTheDeskAction != null)
                {
                    underTheDeskAction.StartAction();
                    StartCoroutine(WaitForActionComplete(underTheDeskAction));
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
                    print("현재 상태: HighlightObject");
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
        print("이제 대사창 띄움~");
        OnActionComplete?.Invoke();
    }
    
    private void SetOption()
    {
        for (int i = 0; i < currentDialog.choices.Length; i++)
        {
            if (UIManager.Instance.optionUI[i])
            {
                UIManager.Instance.SetOptionUI();
            }
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(beforeDialog.nextScene);
    }
}
