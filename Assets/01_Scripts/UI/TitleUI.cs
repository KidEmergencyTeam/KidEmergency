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
}
