using UnityEngine;
using UnityEngine.UI;

public class MapUI : OutlineHighlight
{
    // 멀티가 없어지면 singeButton / multiButton 없이 _button = GetComponent로 진행
    
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
    
    private void ButtonClicked()
    {
        if (UIManager.Instance.titleUI.currentMenu == "Normal" && this.transform.name == _mapName)
        {
            SetCurrentPopup();
        }
        
        else if (UIManager.Instance.titleUI.currentMenu == "Hard" && this.transform.name == _mapName)
        {
            SetCurrentPopup();
        }
    }

    private void SetCurrentPopup()
    {  
        UIManager.Instance.SetPopup(_popupTexts[0], _popupTexts[1], _popupTexts[2]);
        UIManager.Instance.popupUI.TextHighlight();
        UIManager.Instance.titleUI.nextScene = _nextScene;
        UIManager.Instance.popupUI.gameObject.SetActive(true);
    }
}
