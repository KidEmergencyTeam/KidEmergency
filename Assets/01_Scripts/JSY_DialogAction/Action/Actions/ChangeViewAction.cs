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
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());

        while (camOffset.transform.position != newPos)
        {
            camOffset.transform.position = newPos;
            
            yield return StartCoroutine(FadeInOut.Instance.FadeIn());
            _isComplete = true;
        }
    }
    
}
