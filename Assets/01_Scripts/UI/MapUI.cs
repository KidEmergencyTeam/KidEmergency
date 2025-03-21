using System;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : OutlineHighlight
{
    [Tooltip("오브젝트의 이름을 적어주세요.")]
    [SerializeField] private string _mapName;
    [Tooltip("0번 전체 텍스트, 1번 난이도, 2번 모드 순으로 적어주세요.")]
    [TextArea] [SerializeField] private string[] _popupTexts; 
    [Tooltip("게임 시작시 이동할 씬 이름을 적어주세요.")]
    [SerializeField] private string _nextScene;

    private Button _button;


    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClicked);
    }

    private void Update()
    {
        _button.interactable = !TitleUI.Instance.IsPopupOpen();
    }

    private void ButtonClicked()
    {
        if (this.transform.name == _mapName)
        {
            SetCurrentPopup();
        }
        
        else if (this.transform.name == _mapName)
        {
            SetCurrentPopup();
        }
    }

    private void SetCurrentPopup()
    {  
        TitleUI.Instance.SetPopup(_popupTexts[0], _popupTexts[1], _popupTexts[2]);
        TitleUI.Instance.popup.TextHighlight();
        TitleUI.Instance.nextScene = _nextScene;
        TitleUI.Instance.popup.gameObject.SetActive(true);
    }
}
