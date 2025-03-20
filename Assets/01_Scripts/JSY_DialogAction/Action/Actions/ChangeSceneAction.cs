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
    
    private IEnumerator ChangeScene()
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene, LoadSceneMode.Single);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeInOut.Instance.FadeIn());
        _isComplete = true;
    }
    
    
    
}
