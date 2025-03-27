using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameAction : MonoBehaviour, IActionEffect
{
    public bool IsActionComplete { get; }

    public void StartAction()
    {
        BackToStartScene();
    }

    private void BackToStartScene()
    {
        Destroy(ActionManager.Instance.gameObject);
        Destroy(FadeInOut.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

}
