using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameAction : MonoBehaviour, IActionEffect
{
    public bool IsActionComplete { get; }

    public void StartAction()
    {
        StartCoroutine(BackToStartScene());
    }

    private IEnumerator BackToStartScene()
    {
        yield return StartCoroutine(OVRScreenFade.Instance.Fade(0f, 1f));

        Destroy(ActionManager.Instance.gameObject);
        Destroy(FadeInOut.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

}
