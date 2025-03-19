using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public List<MenuUI> menus;
    public string currentMenu; // 현재 선택중인 메뉴
    public string nextScene; // 이동할 씬 이름
    
    public GameObject normalPanel;
    public GameObject hardPanel;
    
    public Button[] buttons;

    private void Start()
    {
        currentMenu = "Normal";
    }

    public void SetCurrentMenuUI()
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
