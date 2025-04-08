using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class OpenCBAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _circuitBox;
    [SerializeField] private GameObject _highlighter;
    
    private bool _isComplete = false; 
    public bool isButtonTriggered = false;
    
    public bool IsActionComplete => _isComplete;

    public void StartAction()
    {
        _isComplete = false;
        isButtonTriggered = false;
        StartCoroutine(OpenCircuitBox());
    }

    private IEnumerator OpenCircuitBox()
    {
        _highlighter.SetActive(true);
        BaseOutlineObject outline = GetComponent<BaseOutlineObject>();
        outline.enabled = true;
        
        while (!_isComplete)
        {
            print(isButtonTriggered);
            
            if (isButtonTriggered)
            {
                print("뚜껑 위치 바뀐다~");

                _circuitBox.transform.localPosition = new Vector3(0, -0.02f, 0.15f);
                _circuitBox.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                print("뚜껑 위치 바꼈따~");

                _highlighter.SetActive(false);
                _isComplete = true;
            }
            
            yield return null;
        }
    }
}
