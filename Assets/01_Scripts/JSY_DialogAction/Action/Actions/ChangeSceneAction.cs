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
        StartCoroutine(FadeInOut.Instance.FadeOut());
        yield return new WaitForSeconds(1.5f);
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        StartCoroutine(FadeInOut.Instance.FadeIn());
        yield return new WaitForSeconds(1.5f);
        _isComplete = true;
    }
    
    
    
}
