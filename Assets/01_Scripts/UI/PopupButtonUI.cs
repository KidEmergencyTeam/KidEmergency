using UnityEngine;
using UnityEngine.UI;

public class PopupButtonUI : OutlineHighlight
{
    private Button _button;

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
#endif
                Application.Quit();
            }
            else
            {
                StartCoroutine(TitleUI.Instance.ChangeScene());
            }
        }
        
        else if (this.transform.name == "No")
        {
            TitleUI.Instance.popup.gameObject.SetActive(false);
        }
    }
    
}
