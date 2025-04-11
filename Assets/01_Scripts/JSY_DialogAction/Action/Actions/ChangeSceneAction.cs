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
        while (!_isComplete)
        {
            if (!UIManager.Instance.warningUI.gameObject.activeSelf)
            {
                yield return StartCoroutine(OVRScreenFade.Instance.Fade(0f, 1f));
        
                AsyncOperation asyncChange = SceneManager.LoadSceneAsync(ActionManager.Instance.beforeDialog.nextScene, LoadSceneMode.Single);
        
                while(!asyncChange.isDone)
                {
                    yield return null;
                }
        
                _isComplete = true;
            }   
        }
    }
    
    
    
}
