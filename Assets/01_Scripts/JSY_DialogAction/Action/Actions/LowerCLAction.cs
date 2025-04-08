using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class LowerCLAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _lever;
    [SerializeField] private GameObject _highlighter;
    [SerializeField] private AudioClip audio;
    
    private bool _isComplete = false;
    public bool isLeverTriggered = false;
    
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
        isLeverTriggered = false;
        StartCoroutine(LowerCircuitLever());
    }

    private IEnumerator LowerCircuitLever()
    {
        _highlighter.SetActive(true);
        BaseOutlineObject outline = GetComponent<BaseOutlineObject>();
        outline.enabled = true;
        
        while (!_isComplete)
        {
            print(isLeverTriggered);
            
            if (isLeverTriggered)
            {
                if (audio != null)
                { 
                    ActionManager.Instance.actionAudio.clip = audio;
                    ActionManager.Instance.actionAudio.Play();
                } 
                print("레버 위치 바뀐다~");
                _lever.transform.localPosition = new Vector3(0, 0.093f, 0.08f);
                _lever.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                print("레버 위치 바ㅏ꼈다~");
                _highlighter.SetActive(false);
                _isComplete = true;
            }
            
            yield return null;
        }
    }
    
}
