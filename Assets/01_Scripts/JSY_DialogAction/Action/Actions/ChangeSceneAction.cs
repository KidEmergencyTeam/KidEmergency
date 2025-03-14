using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAction : MonoBehaviour, IActionEffect
{
    private bool _isComplete;
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(ChangeScene());
    }

    public void StartMultiModeAction()
    {
        
    }
    
    private IEnumerator ChangeScene()
    {
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene);

        // 이렇게 하거나 bags 스크립트가 붙은 가방 프리팹 자체를 삭제하거나
        // if (ActionManager.Instance.beforeDialog.nextScene == "JSY_SchoolGround")
        // {
        //     GameObject obj = GameObject.Find("CamParent").transform.GetChild(1).gameObject;
        //     Destroy(obj);
        // }
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        _isComplete = true;
    }
    
    
    
}
