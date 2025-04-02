using UnityEngine;
using System.Collections;

public class OpenCBAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _circuitBox;

    private Vector3 _openPos = new Vector3(0, -0.02f, 0.15f);
    private Vector3 _openRot = new Vector3(-90, 0, 0);
    private bool _isComplete = false;
    private bool _isTriggered = false;
    
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        StartCoroutine(OpenCircuitBox());
    }

    private IEnumerator OpenCircuitBox()
    {
        while (!_isComplete)
        {
            if (_isTriggered)
            {
                _circuitBox.transform.position = _openPos;
                _circuitBox.transform.rotation = Quaternion.Euler(_openRot);
                _isComplete = true;
            }
            yield return null;
        }
    }

    public void TriggerBox()
    {
        if (!_isComplete)
        {
            _isTriggered = true;
        }
    }
}
