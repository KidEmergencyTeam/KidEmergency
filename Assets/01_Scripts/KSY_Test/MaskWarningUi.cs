using UnityEngine;

public class MaskWarningUi : MonoBehaviour
{
    [Header("경고창")]
    public GameObject maskWarningPanel;

    [Header("FireEvacuationMask")]
    public FireEvacuationMask fireEvacuationMask;

    private void Start()
    {
        // FireEvacuationMask 이벤트에 핸들러 등록
        if (fireEvacuationMask != null)
        {
            // 손수건 충돌
            fireEvacuationMask.OnHandkerchiefEnter += HandleEnter;

            // 손수건 충돌 종료
            fireEvacuationMask.OnHandkerchiefExit += HandleExit;
        }
        else
        {
            Debug.LogError("FireEvacuationMask.cs -> null");
        }

        // 기본적으로 패널은 비활성화
        if (maskWarningPanel != null)
        {
            maskWarningPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("maskWarningPanel -> null");
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandleEnter()
    {
        // 패널 비활성화
        maskWarningPanel.SetActive(false);
    }

    // 손수건과 충돌 종료할 때 실행
    private void HandleExit()
    {
        // 패널 활성화
        maskWarningPanel.SetActive(true);
    }

    // 해당 객체 삭제
    public void OnDestroy()
    {
        // 언제까지 유지하는데???
    }
}
