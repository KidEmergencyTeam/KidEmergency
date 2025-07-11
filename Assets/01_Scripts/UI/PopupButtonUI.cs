using UnityEngine;
using UnityEngine.UI;

public class PopupButtonUI : OutlineHighlight
{
    private Button _button;
    [HideInInspector] public bool isPopup = true;
    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        if (this.transform.name == "Yes")
        {
            if (TitleUI.Instance.currentMenu == "Exit")
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
                Application.Quit();
#else
                Application.Quit();
#endif
            }
            
            else
            {
                if (TitleUI.Instance.nextScene != "")
                {
                    StartCoroutine(TitleUI.Instance.ChangeScene());
                    PopupButtonUI[] bt = FindObjectsOfType<PopupButtonUI>();
                    for (int i = 0; i < bt.Length; i++)
                    {
                        print(bt[i].transform.name);
                        bt[i]._button.interactable = false;
                    }
                }

                else
                {
                    print("nextScene null");
                } 
            }
        }
        
        else if (this.transform.name == "No")
        {
            if (TitleUI.Instance.currentMenu == "Exit")
            {                  
                if (TitleUI.Instance.hardPanel.activeSelf)
                {
                    TitleUI.Instance.currentMenu = "Hard";
                }
                else if (TitleUI.Instance.normalPanel.activeSelf)
                {
                    TitleUI.Instance.currentMenu = "Normal";
                }
                                                        
                TitleUI.Instance.popup.gameObject.SetActive(false);
                outline.color = _originalOutlineColor;
            }

            else
            {
                TitleUI.Instance.popup.gameObject.SetActive(false);
                outline.color = _originalOutlineColor;
            }

        }
    }
    
}
