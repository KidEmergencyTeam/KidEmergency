using System;
using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BaseOutlineObject : MonoBehaviour
{
     public Highlighter _highlighter;

     [SerializeField] private GameObject _leftHand;
     [SerializeField] private GameObject _rightHand;
     
    private void Update()
    {
        if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.1f
           || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.1f)
        {
            _highlighter.SetColor(Color.green);
            _highlighter.isBlinking = false;
        }

        else
        {
            _highlighter.SetColor(Color.yellow);
            _highlighter.isBlinking = true;
        }
    }
    
}
