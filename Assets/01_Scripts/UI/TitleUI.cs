using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public static TitleUI Instance { get; private set; }
    
    public PopupUI popup;
    
    public List<MenuUI> menus;
    public string currentMenu; // 현재 선택중인 메뉴
    public string nextScene; // 이동할 씬 이름
    
    public GameObject normalPanel;
    public GameObject hardPanel;

    private RayController _rayController;
    private bool _isLoading;

    private void Awake()
    {
        _rayController = FindObjectOfType<RayController>();
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        hardPanel.SetActive(false);
        currentMenu = "Normal";
    }

    public void SetCurrentMenuUI()
    {
        for (int i = 0; i < menus.Count; i++)
        { 
            if(currentMenu != menus[i].gameObject.name)
            {
                if (currentMenu != "Exit")
                {
                    menus[i].text.color = menus[i].originalColor;
                    menus[i].activeImage.SetActive(false);
                }
            }
        }
    }
    
    public IEnumerator ChangeScene()
    {
        Destroy(_rayController.leftLine.gameObject);
        Destroy(_rayController.rightLine.gameObject);

        if (!_isLoading)
        {
            yield return StartCoroutine(OVRScreenFade.Instance.Fade(0, 1));

            _isLoading = true;
            AsyncOperation asyncChange = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        
            while(!asyncChange.isDone)
            {
                yield return null;
            }
        
            popup.gameObject.SetActive(false);
        }
    }
    
    public void SetPopup(string text, string levelText, string modeText)
    {
        popup.popupText.text = text;
        popup.highlightText[0] = levelText;
        popup.highlightText[1] = modeText;
    }

    public bool IsPopupOpen()
    {
        if (popup.gameObject.activeSelf)
        {
            return true;
        }

        else return false;
    }
}
