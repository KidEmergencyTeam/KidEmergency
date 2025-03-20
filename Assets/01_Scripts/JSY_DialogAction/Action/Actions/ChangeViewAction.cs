using System.Collections;
using UnityEngine;

public class ChangeViewAction : MonoBehaviour, IActionEffect
{
    public GameObject camOffset;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(SetView(ActionManager.Instance.beforeDialog.changePos));
    }
    
    
    private IEnumerator SetView(Vector3 newPos)
    {
        FadeInOut fade = FindObjectOfType<FadeInOut>();

        yield return StartCoroutine(fade.FadeOut());

        while (camOffset.transform.position != newPos)
        {
            camOffset.transform.position = newPos;
            
            yield return StartCoroutine(fade.FadeIn());
            _isComplete = true;
        }
    }
    
}
