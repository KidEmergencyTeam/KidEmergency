using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseGVAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _target; 
    [SerializeField] private GameObject[] _hand; // 0 왼, 1 오
    [SerializeField] private GameObject _highlighter;
    [SerializeField] private XRKnob _knob;
    
    private bool _isComplete = false;

    public bool IsActionComplete => _isComplete;
    
    private void Start()
    {
        _knob.enabled = false;
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(TryCloseGV());
    }

    private IEnumerator TryCloseGV()
    {
        _highlighter.SetActive(true);
        _target.GetComponent<BaseOutlineObject>().enabled = true;
        
        while (!_isComplete)
        {
            if ((Vector3.Distance(_target.transform.position, _hand[0].transform.position) < 0.1f
                || Vector3.Distance(_target.transform.position, _hand[1].transform.position) < 0.1f) && _knob.value < 0.99f)
            {
                _knob.enabled = true;
            }
            
            else if ((Vector3.Distance(_target.transform.position, _hand[0].transform.position) > 0.1f
                || Vector3.Distance(_target.transform.position, _hand[1].transform.position) > 0.1f) && _knob.value < 0.99f)
            {
                _knob.enabled = false;
            }
            
            if (_knob.value >= 0.99f)
            {
                _knob.enabled = false;
                _knob.value = 1f;
                _target.GetComponent<BaseOutlineObject>().enabled = false;
                _highlighter.SetActive(false);

                _isComplete = true;
            }

            yield return null; 
        }
    }
}
