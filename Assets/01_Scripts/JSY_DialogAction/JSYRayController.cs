using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class JSYRayController : MonoBehaviour
{
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;
    [SerializeField] private XRInteractorLineVisual _leftLine;
    [SerializeField] private XRInteractorLineVisual _rightLine;

    private bool isUIActive = false;

    private void Start()
    {
        // 시작할 땐 오른쪽 레이만 활성화
        _leftRay.enabled = false;
        _rightRay.enabled = true;
    }

    private void Update()
    {

        if (UIActive())
        {
            _leftLine.enabled = true;
            _rightLine.enabled = true;
            
            // 오른쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 왼쪽 레이로 스위치
            if (_leftController.selectAction.action.ReadValue<float>() > 0 && _rightRay.enabled)
            {
                SwitchLeftRay();
            }

            // 왼쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 오른쪽 레이로 스위치
            else if (_rightController.selectAction.action.ReadValue<float>() > 0 && _leftRay.enabled)
            {
                SwitchRightRay();
            }
        }

        else
        {
            _leftRay.enabled = true;
            _rightRay.enabled = true;
            
            _leftLine.enabled = false;
            _rightLine.enabled = false;
        }
    }

    private bool UIActive()
    {
        for (int i = 0; i < UIManager.Instance.optionUI.Length; i++)
        {
            if (UIManager.Instance.optionUI[i].gameObject.activeSelf)
            {
                return true;
            }
        }

        if (UIManager.Instance.titleUI.gameObject.activeSelf)
        {
            return true;
        }
        
        // 타이틀 ui도 추가해야 할듯?
        
        return false;
    }
    
    private void SwitchLeftRay()
    {
        _rightRay.enabled = false;
        _leftRay.enabled = true;
    }

    private void SwitchRightRay()
    {
        _leftRay.enabled = false;
        _rightRay.enabled = true;
    }

}
