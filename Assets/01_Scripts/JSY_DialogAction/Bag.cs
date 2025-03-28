using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class Bag : Grabbable
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private GameObject _headObject; // 현재 카메라 오프셋 -> 플레이어 캐릭터 머리 오브젝트로 변경 예정 
    [SerializeField] private ActionBasedController _leftController; // 왼쪽 컨트롤러 오브젝트
    
    private string _sceneName;
    

    protected override void Start()
    {
        base.Start();
        if (SceneManager.GetActiveScene().name == "Eq_School_1")
        {
            isGrabbable = false;
            currentGrabber.currentGrabbedObject = null;
        }
        else if (SceneManager.GetActiveScene().name == "Eq_School_2")
        {
            currentGrabber.currentGrabbedObject = this;
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_3")
        {
            currentGrabber.currentGrabbedObject = this;
        }
    }

    public void BagInteraction()
    { 
        StartCoroutine(ProtectHead());
    }
    
    private void Update()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        if (_sceneName == "Eq_School_4")
        {
            UIManager.Instance.CloseWarningUI();
            Destroy(this.gameObject);
        }
    }

    private IEnumerator ProtectHead()
    {
        isGrabbable = true;
        
        while (!IsGrabbed)
        {
            UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
            UIManager.Instance.OpenWarningUI();
            
            yield return null;
        }

        while (_sceneName != "Eq_School_4" && IsGrabbed)
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
        if (Vector3.Distance(this.transform.localPosition, _headObject.transform.localPosition) < 0.2f)
        {
            return true;
        }

        else return false;
    }
}
