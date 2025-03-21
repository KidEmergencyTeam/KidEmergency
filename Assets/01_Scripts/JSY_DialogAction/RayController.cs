using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayController : MonoBehaviour
{
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;
    [SerializeField] private XRInteractorLineVisual _leftLine;
    [SerializeField] private XRInteractorLineVisual _rightLine;

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
            if (_leftRay.enabled && _rightRay.enabled)
            {
                _rightRay.enabled = true;
                _rightLine.enabled = true;
                
                _leftRay.enabled = false;
                _leftLine.enabled = false;
            }

            else
            {
                if (_leftRay.enabled)
                {
                    _leftLine.enabled = true;
                    // 왼쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 오른쪽 레이로 스위치
                    if (_rightController.selectAction.action.ReadValue<float>() > 0 && _leftRay.enabled)
                    {
                        SwitchRightRay();
                    }
                }

                else
                {
                    _rightLine.enabled = true;
                    // 오른쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 왼쪽 레이로 스위치
                    if (_leftController.selectAction.action.ReadValue<float>() > 0 && _rightRay.enabled)
                    {
                        for (int i = 0; i < _leftController.transform.childCount; i++)
                        {
                            if (_leftController.transform.GetChild(i).name == "Bag")
                            {
                                return;
                            }

                            if(_leftController.transform.GetChild(i).name != "Bag")
                            {
                                SwitchLeftRay();
                            }
                        }
                    }
                }
            }
        }

        
        else
        {
            for (int i = 0; i < _leftController.transform.childCount; i++)
            {
                if (_leftController.transform.GetChild(i).name == "Bag")
                {
                    _leftRay.enabled = false;
                    _leftLine.enabled = false;
                    _rightLine.enabled = false;
                }

                else
                {
                    _leftLine.enabled = false;
                    _rightLine.enabled = false;
                }
            }
        }
    }

    private bool UIActive()
    {
        if (UIManager.Instance != null)
        {
            for (int i = 0; i < UIManager.Instance.optionUI.Length; i++)
            {
                if (UIManager.Instance.optionUI[i].gameObject.activeSelf)
                {
                    return true;
                }
            }
        }

        if (TitleUI.Instance != null)
        {
            if (TitleUI.Instance.gameObject.activeSelf)
            {
                return true;
            }   
        }
        
        // 타이틀 ui도 추가해야 할듯?
        
        return false;
    }
    
    private void SwitchLeftRay()
    {
        _rightRay.enabled = false;
        _leftRay.enabled = true;
        
        _rightLine.enabled = false;
        _leftLine.enabled = true;
    }

    private void SwitchRightRay()
    {
        _leftRay.enabled = false;
        _rightRay.enabled = true;
        
        _leftLine.enabled = false;
        _rightLine.enabled = true;
    }

}
