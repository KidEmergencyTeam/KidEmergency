using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class Bag : MonoBehaviour
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private GameObject _headObject; // 현재 카메라 오프셋 -> 플레이어 캐릭터 머리 오브젝트로 변경 예정 
    [SerializeField] private ActionBasedController _leftController; // 왼쪽 컨트롤러 오브젝트
    
    // private Grabbable _grab;
    private bool _isGrab = false;
    private string _sceneName;

    private void Awake()
    {
        // _grab = GetComponent<Grabbable>();
    }

    public void BagInteraction()
    { 
        StartCoroutine(ProtectHead());
    }
    
    private void Update()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        if (_sceneName == "JSY_SchoolGround")
        {
            UIManager.Instance.CloseWarningUI();
            Destroy(this.gameObject);
        }
    }

    private IEnumerator ProtectHead()
    {
        // while (!_grab.IsGrabbed)
        // {
        //     UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
        //     UIManager.Instance.OpenWarningUI();
        //
        //     print($"가방 위치: {this.transform.position}, 컨트롤러 위치: {_leftController.transform.position}");
        //     
        //     yield return null;
        // }

        while (!_isGrab)
        {
            if (Vector3.Distance(this.transform.position, _leftController.transform.position) < 0.1f &&
                _leftController.selectAction.action.ReadValue<float>() > 0)
            {
                UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
                UIManager.Instance.OpenWarningUI();
                
                this.transform.SetParent(_leftController.transform);
                this.transform.localPosition = Vector3.zero;
                this.transform.localRotation = Quaternion.Euler(0,0,-90f);
                _isGrab = true;
            }
            
            yield return null;
        }
        
        // while (_sceneName != "JSY_SchoolGround" && _grab.IsGrabbed)
        // {
        //     if (!IsProtect())
        //     {
        //         UIManager.Instance.OpenWarningUI();
        //     }
        //
        //     else
        //     {
        //         UIManager.Instance.CloseWarningUI();
        //     }
        //     
        //     yield return null;
        // }
        
        while (_sceneName != "JSY_SchoolGround" && _isGrab)
        {
            if (!IsProtect())
            {
                UIManager.Instance.OpenWarningUI();
            }

            else
            {
                UIManager.Instance.CloseWarningUI();
            }
            
            yield return null;
        }
    }

    public bool IsProtect()
    {
        if (Vector3.Distance(this.transform.position, _headObject.transform.position) < 0.1f)
        {
            return true;
        }

        else return false;
    }
}
