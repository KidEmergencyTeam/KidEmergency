using System;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BaseOutlineObject : MonoBehaviour
{
     private Outlinable _outlinable;
     private Color _originColor;
     
     [SerializeField] private ActionBasedController _leftCtrl;
     [SerializeField] private ActionBasedController _rightCtrl;
     

    private void Start()
    {
        _outlinable = GetComponent<Outlinable>();
        _originColor = _outlinable.OutlineParameters.Color;
        FindComponents();
    }

    private void Update()
    {
        if(Vector3.Distance(this.gameObject.transform.position, _leftCtrl.transform.position) < 0.1f
           || Vector3.Distance(this.gameObject.transform.position, _rightCtrl.transform.position) < 0.1f)
        {
            _outlinable.OutlineParameters.Color = Color.green;
        }

        else
        {
            _outlinable.OutlineParameters.Color = _originColor;
        }
    }

    private void FindComponents()
    {
        GameObject left = GameObject.Find("Left Controller");
        GameObject right = GameObject.Find("Right Controller");

        print(left);
        print(right);
        
        _leftCtrl = left.gameObject.GetComponentInChildren<ActionBasedController>();
        _rightCtrl = right.gameObject.GetComponentInChildren<ActionBasedController>();
        
        print(_leftCtrl);
        print(_rightCtrl);
    }
}
