using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionManager : SingletonManager<ActionManager>
{    
    public ActionType currentAction;
    public Dialog beforeDialog;
    public Dialog currentDialog;

    public GameObject testPrefab;
    public GameObject testPrefab2;
    public GameObject testPrefab3;

    public event Action OnActionComplete; // 다이얼로그 UI로 상태변경하는 이벤트

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
        print("상태 업데이트 성공");
        switch (currentAction)
        {
            case ActionType.Basic:
                break;
            
            case ActionType.ShowDialog:
                StartCoroutine(DialogManager.Instance.ShowDialog());
                break;
            
            case ActionType.ShowOption:
                SetNewOption();
                if (currentDialog.nextDialog != null)
                {
                    OnActionComplete?.Invoke();
                }
                break;
            
            case ActionType.ChangeScene:
                SetChangeScene();
                Invoke("ActionCompleted", 2f);
                break;
            
            case ActionType.Earthquake:
                Test();
                Invoke("ActionCompleted", 2f);
                break;
            
            case ActionType.UnderTheDesk:
                Test2();
                Invoke("ActionCompleted", 2f);
                break;
            
            case ActionType.PlaceObject:
                Test3();
                Invoke("ActionCompleted", 2f);
                break;
            
            case ActionType.HighlightBag:
                break;
            
            case ActionType.FixingBag:
                break;
        }
        
        // Invoke의 time은 액션의 시간에 따라 변경되어야 함
    }

    private void ActionCompleted()
    {
        OnActionComplete?.Invoke();
    }

    private void SetNewOption()
    {
        for (int i = 0; i < currentDialog.choices.Length; i++)
        {
            if (UIManager.Instance.optionUI[i])
            {
                UIManager.Instance.SetOptionUI();
            }
        }
    }

    private void SetChangeScene()
    {
        SceneManager.LoadScene(beforeDialog.nextScene);
    }
    
    private void Test()
    {
        Instantiate(testPrefab);
    }

    private void Test2()
    {
        Instantiate(testPrefab2);
    }

    private void Test3()
    {
        Instantiate(testPrefab3);
    }

}
