using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : OutlineHighlight
{
    public TextMeshProUGUI text;
    public GameObject activeImage;
    public Color originalColor;
    public string menuName;
    private Button _button;
    

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        activeImage = this.transform.GetChild(1).gameObject;
        originalColor = text.color;
        activeImage.SetActive(false);
        menuName = this.transform.name;
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(MenuButtonClicked);
    }
    
    private void MenuButtonClicked()
    {
        text.color = Color.white;
        activeImage.SetActive(true);

        TitleUI titleUI = GameObject.FindObjectOfType<TitleUI>();
        titleUI.currentMenu = menuName;
        titleUI.SetCurrentMenu();
    }

}
