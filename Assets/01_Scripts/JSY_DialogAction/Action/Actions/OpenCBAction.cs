using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class OpenCBAction : MonoBehaviour, IActionEffect
{
    [SerializeField] private GameObject _cb;
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
        print("두꺼비 오픈 코루틴 시작 ~");
        _highlighter.SetActive(true);
        BaseOutlineObject outline = _cb.GetComponent<BaseOutlineObject>();
        outline.enabled = true;
        
        print($"두꺼비 오픈 와일문 직전! isComplete는 {_isComplete}~ ");
        while (!_isComplete)
        {
            print($"와일 문 들어왔다~ isButtonTriggered {isButtonTriggered}~ ");
            if (isButtonTriggered)
            {
                print("이프문 들어옴! 뚜껑 위치 바뀐다~");
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
