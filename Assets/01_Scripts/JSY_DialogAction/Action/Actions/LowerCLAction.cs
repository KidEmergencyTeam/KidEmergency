using UnityEngine;
using System.Collections;

public class LowerCLAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _lever;
    [SerializeField] private AudioClip audio;

    private Vector3 _lowPos = new Vector3(0, 0.093f, 0.08f);
    private Vector3 _lowRot = new Vector3(90, 0, 0);
    private bool _isComplete = false;
    private bool _isTriggered = false;
    
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
        _isTriggered = false;
        StartCoroutine(LowerCircuitLever());
    }

    private IEnumerator LowerCircuitLever()
    {
        while (!_isComplete)
        {
            if (_isTriggered)
            {
                ActionManager.Instance.actionAudio.clip = audio;
                ActionManager.Instance.actionAudio.Play();
                _lever.transform.position = _lowPos;
                _lever.transform.rotation = Quaternion.Euler(_lowRot);
                _isComplete = true;
            }
            yield return null;
        }
    }

    public void TriggerLever()
    {
        if (!_isComplete)
        {
            _isTriggered = true;
        }
    }
    
}
