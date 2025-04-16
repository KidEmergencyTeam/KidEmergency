using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class BaseOutlineObject : MonoBehaviour
{
     public Highlighter _highlighter;
     public CircuitTrigger box;
     public CircuitTrigger lever;

     public XRRayInteractor _leftRay;
     public XRRayInteractor _rightRay;
     [SerializeField] private GameObject _leftHand;
     [SerializeField] private GameObject _rightHand;
     
     
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Eq_Home_2")
        {
            if (ActionManager.Instance.currentDialog.name == "EqHome22_Dialog" && ActionManager.Instance.currentAction == ActionType.ShowDialog)
            {
                // if (_leftRay.hasHover || _rightRay.hasHover)
                // {
                //     _highlighter.SetColor(Color.green);
                //     _highlighter.isBlinking = false;
                // }
                // else
                // {
                _highlighter.gameObject.SetActive(true);
                _highlighter.SetColor(Color.yellow);
                _highlighter.isBlinking = true; 
                // }   
            }

            else
            {
                _highlighter.gameObject.SetActive(false);
            }
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_Home_1")
        {
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

            else if(ActionManager.Instance.currentAction == ActionType.CloseGasValve)
            {
                if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.08f
                   || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.08f)
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
                if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.15f
                   || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.15f)
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
            if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.15f
               || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.15f)
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
