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
        FadeInOut fade = FindObjectOfType<FadeInOut>();

        yield return StartCoroutine(fade.FadeOut());
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(fade.FadeIn());
        _isComplete = true;
    }
    
    
    
}
