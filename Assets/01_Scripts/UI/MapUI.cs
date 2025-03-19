using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : OutlineHighlight
{
    // 멀티가 없어지면 singeButton / multiButton 없이 _button = GetComponent로 진행
    
    [Tooltip("오브젝트의 이름을 적어주세요.")]
    [SerializeField] private string mapName;
    [Tooltip("0번 전체 텍스트, 1번 난이도, 2번 모드 순으로 적어주세요.")]
    [SerializeField] private string[] popupTexts;
    public Button singleButton;
    public Button multiButton;

    private void Awake()
    {
        singleButton.onClick.AddListener(SingleButtonClicked);
        multiButton.onClick.AddListener(MultiButtonClicked);
    }

    private void SingleButtonClicked()
    {
        if (UIManager.Instance.titleUI.currentMenu == "Normal" && this.transform.name == mapName)
        {
            SetCurrentMode("Single");
        }
        
        else if (UIManager.Instance.titleUI.currentMenu == "Hard" && this.transform.name == mapName)
        {
            SetCurrentMode("Single");
        }
    }

    private void MultiButtonClicked()
    {
        if (UIManager.Instance.titleUI.currentMenu == "Normal" && this.transform.name == mapName)
        {
            SetCurrentMode("Multi");
        }
        
        else if(UIManager.Instance.titleUI.currentMenu == "Hard" && this.transform.name == mapName)

        {
            SetCurrentMode("Multi");
        }
    }

    private void SetCurrentMode(string mode)
    {  
        UIManager.Instance.SetPopup(popupTexts[0], popupTexts[1], popupTexts[2]);
        UIManager.Instance.titleUI.currentMode = mode;
        UIManager.Instance.popupUI.TextHighlight();
        UIManager.Instance.popupUI.gameObject.SetActive(true);
    }
}
