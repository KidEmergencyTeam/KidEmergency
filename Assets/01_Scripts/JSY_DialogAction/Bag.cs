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
        }
        
        else if (SceneManager.GetActiveScene().name == "Eq_School_2" || SceneManager.GetActiveScene().name == "Eq_School_3" || 
                 SceneManager.GetActiveScene().name == "Eq_Home_2")
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
    

    private IEnumerator ProtectHead()
    {
        while (!IsGrabbed)
        {
            UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
            UIManager.Instance.OpenWarningUI();
            
            yield return null;
        }

        while ((_sceneName != "Eq_School_4" || _sceneName != "Eq_Home_3") && IsGrabbed)
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
