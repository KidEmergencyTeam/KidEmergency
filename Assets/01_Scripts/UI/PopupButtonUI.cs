using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            UIManager.Instance.popupUI.gameObject.SetActive(false);
            
            if (UIManager.Instance.titleUI.currentMenu == "Exit")
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
            else
            {
                StartCoroutine(ChangeScene());
            }
        }
        
        else if (this.transform.name == "No")
        {
            UIManager.Instance.popupUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator ChangeScene()
    {
        UIManager.Instance.titleUI.gameObject.SetActive(false);
        
        StartCoroutine(FadeInOut.Instance.FadeOut());
        yield return new WaitForSeconds(1.5f);
        
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(UIManager.Instance.titleUI.nextScene);
        
        while(!asyncChange.isDone)
        {
            yield return null;
        }

        StartCoroutine(FadeInOut.Instance.FadeIn());
        yield return new WaitForSeconds(1.5f);
    }
}
