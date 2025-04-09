using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (SceneManager.GetActiveScene().name == "Eq_School_1" ||
            SceneManager.GetActiveScene().name == "Eq_Home_1")
        {
            isGrabbable = false;
            currentGrabber.currentGrabbedObject = null;
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_2")
        {
            Grabber grabber = FindObjectOfType<Grabber>();
            grabber.OnGrab(this);
            isGrabbable = false;
            BagInteraction();
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_3")
        {
            Grabber grabber = FindObjectOfType<Grabber>();
            grabber.OnGrab(this);
            isGrabbable = false;
            BagInteraction();
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_Home_2")
        {
            Grabber grabber = FindObjectOfType<Grabber>();
            grabber.OnGrab(this);
            isGrabbable = false;
            BagInteraction();
        }
    }

    public void BagInteraction()
    { 
        StartCoroutine(ProtectHead());
    }

    protected override void Update()
    {
        base.Update();
        if (SceneManager.GetActiveScene().name == "Eq_School_1"
            || SceneManager.GetActiveScene().name == "Eq_Home_1")
        {
            if (currentGrabber.currentGrabbedObject == this)
            {
                isGrabbable = false;
                
                if (_handObject != null)
                {
                    this.transform.SetParent(_handObject);
                    this.transform.localPosition = new Vector3(0.00022f, 0.00106f, 0.00014f);
                    this.transform.localRotation = Quaternion.Euler(-90, -90, -90);
                    this.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                }
            }
        }
    }

    private IEnumerator ProtectHead()
    {
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
        if (Vector3.Distance(this.transform.position, _headObject.transform.position) < 0.2f)
        {
            return true;
        }
        return false;
    }
}
