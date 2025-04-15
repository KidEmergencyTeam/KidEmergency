using UnityEngine;

public class OptionDialogSwitch : MonoBehaviour
{
    private DialogUI dialogUI;

    private void Start()
    {
        // 태그로 오브젝트를 찾고 DialogUI.cs 가져오기
        dialogUI = GameObject.FindGameObjectWithTag("DialogUI")?.GetComponent<DialogUI>();
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI -> null");
        }
        else
        {
            dialogUI.dialogPanel.SetActive(false);
            Debug.Log("dialogPanel 켜짐");
        }
    }

    // optionUI의 활성화 상태를 체크하여 dialogUI의 활성화 여부를 조절
    void Update()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager -> null");
            return;
        }

        // optionUI 배열에서 활성화된 항목 여부 체크
        bool isOptionActive = false;
        if (UIManager.Instance.optionUI != null)
        {
            foreach (var option in UIManager.Instance.optionUI)
            {
                if (option != null && option.gameObject.activeSelf)
                {
                    isOptionActive = true;
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("optionUI -> null");
        }

        // 활성화된 optionUI가 있으면 dialogUI 비활성화, 그렇지 않으면 활성화
        dialogUI.dialogPanel.SetActive(!isOptionActive);
    }
}
