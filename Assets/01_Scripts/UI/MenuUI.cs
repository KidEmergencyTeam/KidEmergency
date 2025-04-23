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

    private void Update()
    {
        _button.interactable = !TitleUI.Instance.IsPopupOpen();
        
    }

    private void MenuButtonClicked()
    {
        if (_menuName != "Exit")
        {
            text.color = Color.white;
            activeImage.SetActive(true);   
            TitleUI.Instance.currentMenu = _menuName;
        }
        
        TitleUI.Instance.SetCurrentMenuUI();

        if (_menuName == "Normal")
        {
            TitleUI.Instance.hardPanel.SetActive(false);
            TitleUI.Instance.normalPanel.SetActive(true);
        }
        
        else if (_menuName == "Hard")
        {
            TitleUI.Instance.normalPanel.SetActive(false);
            TitleUI.Instance.hardPanel.SetActive(true);
        }
        
        else if (_menuName == "Exit")
        {
            TitleUI.Instance.currentMenu = _menuName;
            TitleUI.Instance.SetPopup("게임을 종료하시겠습니까?", null, null);
            TitleUI.Instance.popup.gameObject.SetActive(true);
        }
    }

}
