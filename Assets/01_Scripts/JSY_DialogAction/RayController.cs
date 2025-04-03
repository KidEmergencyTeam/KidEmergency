using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayController : MonoBehaviour
{
    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;
    [SerializeField] private XRRayInteractor _leftRay;
    [SerializeField] private XRRayInteractor _rightRay;
    [SerializeField] private HandAnimation _handAnim;
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
        Grabber grabber = FindObjectOfType<Grabber>();
        if (UIActive())
        {
            bool isLeft = _leftController.selectAction.action.ReadValue<float>() > 0;
            bool isRight = _rightController.selectAction.action.ReadValue<float>() > 0;
            
            if(isLeft) _handAnim.animator.SetFloat("Left Grip", 1f);
            if(isRight) _handAnim.animator.SetFloat("Right Grip", 1f);

            if (grabber.isOnGrabCalled)
            {
                _rightRay.enabled = true;
                rightLine.enabled = true;
                        
                _leftRay.enabled = false;
                leftLine.enabled = false;
            }

            else
            {
                if (_leftRay.enabled && _rightRay.enabled)
                {
                    _rightRay.enabled = true;
                    rightLine.enabled = true;
                
                    _leftRay.enabled = false;
                    leftLine.enabled = false;
                }

                else if(_leftRay.enabled || _rightRay.enabled)
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

                    else if(_rightRay.enabled)
                    {
                        rightLine.enabled = true;
                        
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
