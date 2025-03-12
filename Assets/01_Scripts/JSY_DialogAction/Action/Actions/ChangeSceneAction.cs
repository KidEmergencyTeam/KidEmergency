using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAction : MonoBehaviour, IActionEffect
{
    private bool isComplete;
    public bool IsActionComplete => isComplete;

    public void StartAction()
    {
        isComplete = false;
        StartCoroutine(ChangeScene());
    }
    
    private IEnumerator ChangeScene()
    {
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene);
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        isComplete = true;
    }
    
    
    
}
