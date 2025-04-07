using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseOutlineObject : MonoBehaviour
{
     public Highlighter _highlighter;

     [SerializeField] private GameObject _leftHand;
     [SerializeField] private GameObject _rightHand;
     
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Eq_School_1")
        {
            if(Vector3.Distance(this.gameObject.transform.position, _leftHand.transform.position) < 0.05f
               || Vector3.Distance(this.gameObject.transform.position, _rightHand.transform.position) < 0.05f)
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
        else if (SceneManager.GetActiveScene().name == "Eq_Home_1")
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
