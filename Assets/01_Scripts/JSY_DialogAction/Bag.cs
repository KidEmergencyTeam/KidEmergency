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
    
    private Grabbable _grab;
    private string _sceneName;

    private void Awake()
    {
        _grab = GetComponent<Grabbable>();
    }

    public void BagInteraction()
    {
        if (_grab.isGrabbable)
        {
            StartCoroutine(ProtectHead());
        }
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
        while (!_grab.IsGrabbed)
        {
            UIManager.Instance.SetWarningUI(_warningSprite, _warningText);
            UIManager.Instance.OpenWarningUI();

            print($"가방 위치: {this.transform.position}, 컨트롤러 위치: {_leftController.transform.position}");
            
            yield return null;
        }
        
        while (_sceneName != "JSY_SchoolGround" && _grab.IsGrabbed)
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
        if (Vector3.Distance(this.transform.localPosition, _headObject.transform.position) < 0.1f)
        {
            return true;
        }

        else return false;
    }
}
