using UnityEngine;

// 주요 UI 위치 및 회전값을 변경 해주는 스크립트
public class UIPosition : MonoBehaviour
{
    // optionPanel
    [Header("optionPanel")]
    public GameObject optionPanel;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 optionPanelChangePosition;
    public Vector3 optionPanelChangelRotation;

    // dialogUI
    [Header("dialogUI")]
    public GameObject dialogUI;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 dialogUIChangePosition;
    public Vector3 dialogUIChangeRotation;

    // warningUI
    [Header("warningUI")]
    public GameObject warningUI;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 warningUIChangePosition;
    public Vector3 warningUIChangeRotation;

    // 다른 스크립트에서 메서드를 호출할 때 각 오브젝트의 위치 및 회전값 적용
    public void UpdatePosition()
    {
        // optionPanel 위치 및 회전값 적용
        if (optionPanel != null)
        {
            optionPanel.transform.position = optionPanelChangePosition;
            optionPanel.transform.eulerAngles = optionPanelChangelRotation;
        }
        else
        {
            Debug.LogError("optionPanel -> null");
        }

        // dialogUI 위치 및 회전값 적용
        if (dialogUI != null)
        {
            dialogUI.transform.position = dialogUIChangePosition;
            dialogUI.transform.eulerAngles = dialogUIChangeRotation;
        }
        else
        {
            Debug.LogError("dialogUI -> null");
        }

        // warningUI 위치 및 회전값 적용
        if (warningUI != null)
        {
            warningUI.transform.position = warningUIChangePosition;
            warningUI.transform.eulerAngles = warningUIChangeRotation;
        }
        else
        {
            Debug.LogError("warningUI -> null");
        }
    }
}
