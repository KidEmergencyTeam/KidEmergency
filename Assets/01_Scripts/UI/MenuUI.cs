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
        activeImage.SetActive(false);
        _menuName = this.transform.name;
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(MenuButtonClicked);
    }
    
    private void MenuButtonClicked()
    {
        text.color = Color.white;
        activeImage.SetActive(true);

        TitleUI titleUI = GetComponentInParent<TitleUI>();
        titleUI.currentMenu = _menuName;
        titleUI.SetCurrentMenu();

        if (_menuName == "Normal")
        {
            
        }
        
        else if (_menuName == "Hard")
        {
            
        }
        
        else if (_menuName != "Exit")
        {
            
        }
    }

}
