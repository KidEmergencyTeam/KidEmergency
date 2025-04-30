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
                if (ActionManager.Instance.beforeDialog.nextScene == "Eq_School_2")
                {
                    UIManager.Instance.DialogPosReset(2);
                    UIManager.Instance.OptionPosReset(1);
                    // UIManager.Instance.WarningPosReset(2);
                }

                if (ActionManager.Instance.beforeDialog.nextScene == "Eq_School_3")
                {
                    UIManager.Instance.DialogPosReset(3);
                    UIManager.Instance.OptionPosReset(2);
                    // UIManager.Instance.WarningPosReset(3);
                }

                if (ActionManager.Instance.beforeDialog.nextScene == "Eq_School_4")
                {
                    UIManager.Instance.DialogPosReset(4);
                    UIManager.Instance.CloseWarningUI();
                }

                if (ActionManager.Instance.beforeDialog.nextScene == "Eq_Home_2")
                {
                    UIManager.Instance.DialogPosReset(4);
                    // UIManager.Instance.WarningPosReset(2);
                    UIManager.Instance.OptionPosReset(1);
                }

                if (ActionManager.Instance.beforeDialog.nextScene == "Eq_Home_3")
                {
                    UIManager.Instance.DialogPosReset(6);
                }
                while(!asyncChange.isDone)
                {
                    yield return null;
                }
        
                _isComplete = true;
            }

            yield return null;
        }
    }
    
    
    
}
