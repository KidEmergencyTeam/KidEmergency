using UnityEngine;

// 주요 UI 위치 및 회전값을 변경 해주는 스크립트
public class UIPosition : MonoBehaviour
{
    // optionPanel
    [Header("optionPanel")]
    public GameObject optionPanel;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 optionPanelInitialPosition;
    public Vector3 optionPanelInitialRotation;

    // dialogUI
    [Header("dialogUI")]
    public DialogUI dialogUI;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 dialogUIInitialPosition;
    public Vector3 dialogUIInitialRotation;

    // warningUI
    [Header("warningUI")]
    public WarningUI warningUI;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 warningUIInitialPosition;
    public Vector3 warningUIInitialRotation;

    // 다른 스크립트에서 메서드를 호출할 때 각 오브젝트의 위치 및 회전값 적용
    public void UpdatePosition()
    {
        // optionPanel 위치 및 회전값 적용
        if (optionPanel != null)
        {
            optionPanel.transform.position = optionPanelInitialPosition;
            optionPanel.transform.eulerAngles = optionPanelInitialRotation;
        }
        else
        {
            Debug.LogError("optionPanel -> null");
        }

        // dialogUI 위치 및 회전값 적용
        if (dialogUI != null)
        {
            dialogUI.transform.position = dialogUIInitialPosition;
            dialogUI.transform.eulerAngles = dialogUIInitialRotation;
        }
        else
        {
            Debug.LogError("dialogUI -> null");
        }

        // warningUI 위치 및 회전값 적용
        if (warningUI != null)
        {
            warningUI.transform.position = warningUIInitialPosition;
            warningUI.transform.eulerAngles = warningUIInitialRotation;
        }
        else
        {
            Debug.LogError("warningUI -> null");
        }
    }
}
