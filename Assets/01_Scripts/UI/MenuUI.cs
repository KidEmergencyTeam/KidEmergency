using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : OutlineHighlight
{
    public TextMeshProUGUI text;
    public GameObject activeImage;
    [HideInInspector] public Color originalColor;
    private string _menuName;
    private Button _button;


    protected override void Awake()
    {
        base.Awake();
        originalColor = text.color;
        _menuName = this.transform.name;
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(MenuButtonClicked);
    }

    private void Start()
    {
        if (this.transform.name == "Normal")
        {
            text.color = Color.white;
        }
    }

    private void MenuButtonClicked()
    {
        if (_menuName != "Exit")
        {
            text.color = Color.white;
            activeImage.SetActive(true);   
        }
        
        UIManager.Instance.titleUI.currentMenu = _menuName;
        UIManager.Instance.titleUI.SetCurrentMenuUI();

        if (_menuName == "Normal")
        {
            UIManager.Instance.titleUI.hardPanel.SetActive(false);
            UIManager.Instance.titleUI.normalPanel.SetActive(true);
        }
        
        else if (_menuName == "Hard")
        {
            UIManager.Instance.titleUI.normalPanel.SetActive(false);
            UIManager.Instance.titleUI.hardPanel.SetActive(true);
        }
        
        else if (_menuName == "Exit")
        {
            UIManager.Instance.SetPopup("게임을 종료하시겠습니까?", null, null);
            UIManager.Instance.popupUI.gameObject.SetActive(true);
        }
    }

}
