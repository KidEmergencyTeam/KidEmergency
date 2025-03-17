using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bags : MonoBehaviour
{
    [SerializeField] private Sprite warningSprite;
    [SerializeField] private string warningText;
    [SerializeField] private GameObject camParent;
    private float interactableRange = 0.5f;
    private string sceneName;


    public void BagInteraction()
    {
        StartCoroutine(ProtectHead());
    }
    private void Update()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "JSY_SchoolGround")
        {
            Destroy(this.gameObject);
            // 플레이어 컨트롤러로 상태도 바꾸기
        }
    }

    private IEnumerator ProtectHead()
    {
        // 씬을 옮겨도 destroy 되지 않게
        this.gameObject.transform.SetParent(camParent.transform);
        
        while (sceneName != "JSY_SchoolGround")
        {
            if (IsProtect())
            {
                UIManager.Instance.SetWarningUI(warningSprite, warningText);
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
        if (Vector3.Distance(this.transform.position, camParent.transform.position) < interactableRange)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
