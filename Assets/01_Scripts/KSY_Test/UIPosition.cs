using UnityEngine;

// 주요 UI 위치 및 회전값을 변경 해주는 스크립트
public class UIPosition : MonoBehaviour
{
    // OptionPos
    [Header("OptionPos")]
    public GameObject optionPos;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 optionPosChangePosition;
    public Vector3 optionPosChangelRotation;

    // DialogPos
    [Header("DialogPos")]
    public GameObject dialogPos;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 dialogPosChangePosition;
    public Vector3 dialogPosChangeRotation;

    // WarningPos
    [Header("WarningPos")]
    public GameObject warningPos;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 warningPosChangePosition;
    public Vector3 warningPosChangeRotation;

    // SetiPos
    [Header("SetiPos")]
    public GameObject setiPos;

    [Header("변경될 위치 및 회전값 할당")]
    public Vector3 setiPosChangePosition;
    public Vector3 setiPosChangeRotation;

    // 다른 스크립트에서 메서드를 호출할 때 각 오브젝트의 위치 및 회전값 적용
    public void UpdatePosition()
    {
        // OptionPos 변경될 위치 및 회전값 적용
        if (optionPos != null)
        {
            optionPos.transform.localPosition = optionPosChangePosition;
            optionPos.transform.localRotation = Quaternion.Euler(optionPosChangelRotation);
        }
        else
        {
            Debug.LogError("OptionPos -> null");
        }

        // DialogPos 변경될 위치 및 회전값 적용
        if (dialogPos != null)
        {
            dialogPos.transform.localPosition = dialogPosChangePosition;
            dialogPos.transform.localRotation = Quaternion.Euler(dialogPosChangeRotation);
        }
        else
        {
            Debug.LogError("DialogPos -> null");
        }

        // WarningPos 변경될 위치 및 회전값 적용
        if (warningPos != null)  
        {
            warningPos.transform.localPosition = warningPosChangePosition;
            warningPos.transform.localRotation = Quaternion.Euler(warningPosChangeRotation);
        }
        else
        {
            Debug.LogError("WarningPos -> null");
        }

        // setiPos 변경될 위치 및 회전값 적용
        if (setiPos != null)
        {
            setiPos.transform.position = setiPosChangePosition;
            setiPos.transform.rotation = Quaternion.Euler(setiPosChangeRotation);
        }
        else
        {
            Debug.LogError("setiPos -> null");
        }
    }
}
