using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class BaseOutlineObject : MonoBehaviour
{
     public Highlighter _highlighter;
     public CircuitTrigger box;
     public CircuitTrigger lever;

     [SerializeField] private XRRayInteractor _leftRay;
     [SerializeField] private XRRayInteractor _rightRay;
     [SerializeField] private GameObject _leftHand;
     [SerializeField] private GameObject _rightHand;
     
     
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Eq_Home_2")
        {
            if (_leftRay != null && _rightRay != null)
            {
                if (_leftRay.hasHover || _rightRay.hasHover)
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
        
        else if (SceneManager.GetActiveScene().name == "Eq_Home_1")
        {
            if (ActionManager.Instance.currentAction == ActionType.CloseGasValve)
            {
                if (_leftRay != null && _rightRay != null)
                {
                    if (_leftRay.hasHover || _rightRay.hasHover)
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
            
            if (ActionManager.Instance.currentAction == ActionType.OpenCircuitBox)
            {
                if (box.isTriggered)
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
            
            else if (ActionManager.Instance.currentAction == ActionType.LowerCircuitLever)
            {
                if (lever.isTriggered)
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

            else
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
        
        else
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
    
}
