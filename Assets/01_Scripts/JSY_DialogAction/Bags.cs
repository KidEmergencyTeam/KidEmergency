using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Bags : MonoBehaviour
{
    [SerializeField] private Sprite _warningSprite;
    [SerializeField] private string _warningText;
    [SerializeField] private GameObject _player; // 카메라 오프셋
    private float _interactableRange = 0.5f;
    private string _sceneName;


    public void BagInteraction()
    {
        StartCoroutine(ProtectHead());
    }
    
    private void Update()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        if (_sceneName == "JSY_SchoolGround")
        {
            Destroy(this.gameObject);
            // 플레이어 컨트롤러로 상태도 바꾸기
        }
    }

    private IEnumerator ProtectHead()
    {
        // 씬을 옮겨도 destroy 되지 않게
        this.gameObject.transform.SetParent(_player.transform);
        
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
        if (Vector3.Distance(this.transform.position, _player.transform.position) < _interactableRange)
        {
            return true;
        }

        else return false;
    }
}
