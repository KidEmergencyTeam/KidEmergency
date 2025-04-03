using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayController : MonoBehaviour
{
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;
    public XRInteractorLineVisual leftLine;
    public XRInteractorLineVisual rightLine;

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
                rightLine.enabled = true;
                
                _leftRay.enabled = false;
                leftLine.enabled = false;
            }

            else
            {
                if (_leftRay.enabled)
                {
                    leftLine.enabled = true;
                    // 왼쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 오른쪽 레이로 스위치
                    if (_rightController.selectAction.action.ReadValue<float>() > 0 && _leftRay.enabled)
                    {
                        SwitchRightRay();
                    }
                }

                else
                {
                    Grabber grabber = GetComponentInChildren<Grabber>();
                    rightLine.enabled = true;

                    // 오브젝트를 그랩하고 있는 상태라면 레이 스위치 X, 무조건 오른 손에만 Ray 활성화
                    if (grabber.isOnGrabCalled)
                    {
                        _rightRay.enabled = true;
                        rightLine.enabled = true;
                        
                        _leftRay.enabled = false;
                        leftLine.enabled = false;
                    }

                    // 오른쪽 레이가 켜져있는 상태에서 왼쪽 컨트롤러의 그립 버튼을 눌렀으면 왼쪽 레이로 스위치
                    else
                    {
                        if (_leftController.selectAction.action.ReadValue<float>() > 0 && _rightRay.enabled)
                        {
                            SwitchLeftRay();
                        }
                    }
                }
            }
        }

        else
        {
            Grabber grabber = GetComponentInChildren<Grabber>();
            if (grabber.isOnGrabCalled)
            {
                _rightRay.enabled = true;
                _leftRay.enabled = false;
                
                leftLine.enabled = false;
                rightLine.enabled = false;
            }

            else
            {
                _leftRay.enabled = true;
                _rightRay.enabled = true;

                leftLine.enabled = false;
                rightLine.enabled = false;
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
        
        return false;
    }
    
    private void SwitchLeftRay()
    {
        _rightRay.enabled = false;
        _leftRay.enabled = true;
        
        rightLine.enabled = false;
        leftLine.enabled = true;
    }

    private void SwitchRightRay()
    {
        _leftRay.enabled = false;
        _rightRay.enabled = true;
        
        leftLine.enabled = false;
        rightLine.enabled = true;
    }

}
