using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeViewAction : MonoBehaviour, IActionEffect
{
    // 멀티 모드일 시 스폰 위치에 따라 시점 변경되므로 스크립트 내용도 변경돼야함
    
    public float fadeDuration = 1.5f;
    public Image fadeEffect;
    public GameObject xrCamParent;
    
    private Coroutine _currentCoroutine;
    private bool _isComplete = false;
    public bool IsActionComplete => _isComplete;
    
    public void StartAction()
    {
        _isComplete = false;
        ViewChange();
    }
    
    private void ViewChange()
    {
        _currentCoroutine = StartCoroutine(Fade(0,1,fadeDuration)); 
        StartCoroutine(WaitForAction());
    }

    private IEnumerator WaitForAction()
    {
        yield return _currentCoroutine;
        SetView(ActionManager.Instance.beforeDialog.viewValue);
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
    
    private void SetView(float value)
    {
        Vector3 changeCameraPos = xrCamParent.transform.position;
        changeCameraPos.y += value; // 씬에서 테스트 후 값 변경하면 됨
        xrCamParent.transform.position = changeCameraPos;
    }
    
}
