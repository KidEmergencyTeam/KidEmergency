using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EqHomeTrigger : MonoBehaviour
{
    private Vector3 _originPos;
    private Vector3 _originRot;
    private bool _isTouch = false;
    private GameObject _target;

    public bool isOpen = false;
    public bool isDown = false;
    public Vector3 nextPos;
    public Vector3 nextRot;
    
    private void Start()
    {
        _target = this.gameObject;
        _originPos = _target.transform.position;
        _originRot = _target.transform.rotation.eulerAngles;
    }

    public void Corou()
    {
        StartCoroutine(SettingComplete());
    }
    
    private IEnumerator SettingComplete()
    {
        if (_target.name == "FuseBox")
        {
            if (_isTouch)
            {
                SetPosRot();
                isOpen = true;

                yield return null;
            }

            else
            {
                isOpen = false;
            }
        }
        
        else if (_target.name == "Lever")
        {
            if (_isTouch)
            {
                SetPosRot();
                isDown = true;
            
                yield return null;
            }

            else
            {
                isDown = false;
            }
        }
    }
    
    private void SetPosRot()
    {
        transform.position = nextPos; 
        transform.rotation = Quaternion.Euler(nextRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTouch = true;
        }
    }
}
