using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class LowerCLAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _lever;
    [SerializeField] private GameObject _highlighter;
    [SerializeField] private AudioClip audio;
    [SerializeField] private ActionBasedController[] _controller;
    [SerializeField] private Collider[] _hand;
    
    private bool _isComplete = false;
    public bool isLvLeftTrigger = false;
    public bool isLvRightTrigger = false;
    public bool IsActionComplete => _isComplete;

    private void Start()
    {
        for (int i = 0; i < _hand.Length; i++)
        {
            _hand[i].enabled = false;
        }
    }
    
    public void StartAction()
    {
        _isComplete = false;
        isLvLeftTrigger = false;
        isLvRightTrigger = false;
        StartCoroutine(LowerCircuitLever());
    }

    private IEnumerator LowerCircuitLever()
    {
        _highlighter.SetActive(true);
        for (int i = 0; i < _hand.Length; i++)
        {
            _hand[i].enabled = true;
        }
        BaseOutlineObject outline = _lever.GetComponent<BaseOutlineObject>();
        outline.enabled = true;
        
        while (!_isComplete)
        {
            if (isLvLeftTrigger)
            {
                if (_controller[0].selectAction.action.ReadValue<float>() >= 1)
                {
                    if (audio != null)
                    {
                        if (ActionManager.Instance.actionAudio.clip != audio)
                        {
                            ActionManager.Instance.actionAudio.clip = audio;
                            ActionManager.Instance.actionAudio.Play();
                        }
                    } 
                    print("레버 위치 바뀐다~");
                    _lever.transform.localPosition = new Vector3(0, 0.093f, 0.08f);
                    _lever.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    print("레버 위치 바ㅏ꼈다~");
                    _highlighter.SetActive(false);
                    for (int i = 0; i < _hand.Length; i++)
                    {
                        _hand[i].enabled = false;
                    }
                    _isComplete = true;
                }
            }

            if (isLvRightTrigger)
            {
                if (_controller[1].selectAction.action.ReadValue<float>() >= 1)
                {
                    if (audio != null)
                    { 
                        ActionManager.Instance.actionAudio.clip = audio;
                        ActionManager.Instance.actionAudio.Play();
                    } 
                    _lever.transform.localPosition = new Vector3(0, 0.093f, 0.08f);
                    _lever.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    _highlighter.SetActive(false);
                    for (int i = 0; i < _hand.Length; i++)
                    {
                        _hand[i].enabled = false;
                    }
                    _isComplete = true;
                }
            }
            
            
            yield return null;
        }
    }
    
}
