using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public List<MenuUI> menus;
    public string currentMenu;

    public GameObject normalPanel;
    public GameObject hardPanel;
    
    public Button[] buttons;
    
    public void SetCurrentMenu()
    {
        for (int i = 0; i < menus.Count; i++)
        { 
            if(currentMenu != menus[i].gameObject.name)
            {
                menus[i].text.color = menus[i].originalColor;
                menus[i].activeImage.SetActive(false);
            }
        }
    }
}
