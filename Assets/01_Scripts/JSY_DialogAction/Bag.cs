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
    [SerializeField] private Transform _handObject;
    [SerializeField] private GameObject _headObject; // 메인 카메라

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
            BagInteraction();
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_3")
        {
            currentGrabber.currentGrabbedObject = this;
            BagInteraction();
        }
    }

    public void BagInteraction()
    { 
        StartCoroutine(ProtectHead());
    }
    
    private void Update()
    {
        if (currentGrabber.currentGrabbedObject == this)
        {
            isGrabbable = false;
            this.transform.SetParent(_handObject);
            this.transform.localPosition = new Vector3(0.0005f, 0.0008f, -0.0008f);
            this.transform.localRotation = Quaternion.Euler(-90, -90, -90);
            this.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
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
        if (Vector3.Distance(this.transform.position, _headObject.transform.position) < 0.1f)
        {
            return true;
        }

        return false;
    }
}
