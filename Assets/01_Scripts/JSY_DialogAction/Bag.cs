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
    [SerializeField] private GameObject _camOffset; 
    [SerializeField] private ActionBasedController _rightController; // 오른쪽 컨트롤러 오브젝트
    [SerializeField] private ActionBasedController _leftController; // 왼쪽 컨트롤러 오브젝트

    private Rigidbody rb;
    private string _sceneName;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void BagInteraction()
    {
        StartCoroutine(ProtectHead());
    }

    private void Grapped()
    {
        if (ActionManager.Instance.currentAction == ActionType.FixingBag)
        {
            if (Vector3.Distance(this.transform.position, _rightController.transform.position) < 0.3f &&
                _rightController.selectAction.action.ReadValue<float>() > 0)
            {
                this.transform.SetParent(_rightController.transform);
                this.transform.localPosition = Vector3.zero;
                this.transform.localRotation = Quaternion.Euler(0,0,-90f);
                rb.isKinematic = true;
                
            }

            else if (Vector3.Distance(this.transform.position, _leftController.transform.position) < 0.3f &&
                     _leftController.selectAction.action.ReadValue<float>() > 0)
            {
                this.transform.SetParent(_leftController.transform);
                this.transform.localPosition = Vector3.zero;
                this.transform.localRotation = Quaternion.Euler(0,0,-90f);
                rb.isKinematic = true;
            }
        }
    }
    
    private void Update()
    {
        if (this.gameObject != null)
        {
            Grapped();
        }
        
        _sceneName = SceneManager.GetActiveScene().name;
        if (_sceneName == "JSY_SchoolGround")
        {
            Destroy(this.gameObject);
            UIManager.Instance.CloseWarningUI();
            // 플레이어 컨트롤러로 상태도 바꾸기
        }
    }

    private IEnumerator ProtectHead()
    {
        // this.gameObject.transform.SetParent(_player.transform);
        
        while (_sceneName != "JSY_SchoolGround")
        {
            if (!IsProtect())
            {
                UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
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
        if (Vector3.Distance(this.transform.position, _camOffset.transform.position) < 0.2f)
        {
            return true;
        }

        else return false;
    }
}
