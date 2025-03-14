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
    private float interactableRange;


    public void BagInteraction()
    {
        StartCoroutine(ProtectHead());
    }
    private void Update()
    {
        string groundScene = SceneManager.GetActiveScene().name;
        if (groundScene == "JSY_SchoolGround")
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator ProtectHead()
    {
        while (true)
        {
            if (Vector3.Distance(this.transform.position, camParent.transform.position) > interactableRange)
            {
                UIManager.Instance.SetWarningUI(warningSprite, warningText);
                UIManager.Instance.OpenWarningUI();
            }

            else
            {
                UIManager.Instance.CloseWarningUI();
            }
        }
    }
}
