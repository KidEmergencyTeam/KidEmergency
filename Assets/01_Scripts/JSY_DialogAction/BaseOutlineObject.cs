using System;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BaseOutlineObject : MonoBehaviour
{
     private Outlinable _outlinable;
     private Color _originColor;

     [SerializeField] private GameObject _leftHand;
     [SerializeField] private GameObject _rightHand;
     
     private void Start()
    {
        _outlinable = GetComponent<Outlinable>();
        _originColor = _outlinable.OutlineParameters.Color;
    }

    private void Update()
    {
        if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.05f
           || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.05f)
        {
            _outlinable.OutlineParameters.Color = Color.green;
        }

        else
        {
            _outlinable.OutlineParameters.Color = _originColor;
        }
    }
    
}
