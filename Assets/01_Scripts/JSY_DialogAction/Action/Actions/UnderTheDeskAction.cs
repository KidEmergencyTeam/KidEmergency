using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnderTheDeskAction : MonoBehaviour, IActionEffect
{
    public float fadeDuration = 2f;
    public Image fadeEffect;
    public GameObject xrCamParent;
    
    private Coroutine _currentCoroutine;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        AllAction();
    }
    
    private void AllAction()
    {
        _currentCoroutine = StartCoroutine(Fade(0,1,fadeDuration)); 
        StartCoroutine(WaitForAction());
    }

    private IEnumerator WaitForAction()
    {
        yield return _currentCoroutine;
        ChangeView();
        _currentCoroutine = StartCoroutine(Fade(1,0,fadeDuration));
        _isComplete = true;
    }
    
    
    private IEnumerator Fade(float start, float end, float duration)
    {
        float currentTime = 0f;
        float percent = 0f;
        while (percent < 1f)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / duration;
            
            Color color = fadeEffect.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeEffect.color = color;

            yield return null;
        }

        _currentCoroutine = null;
    }
    
    private void ChangeView()
    {
        Vector3 changeCameraPos = xrCamParent.transform.position;
        changeCameraPos.y -= 1f;
        xrCamParent.transform.position = changeCameraPos;
    }
    
}
