using UnityEngine;

// 경고창 ui 관리 스크립트
public class MaskWarningUI : MonoBehaviour
{
    [Header("경고창")]
    public GameObject maskWarningPanel;

    [Header("FireEvacuationMask")]
    public FireEvacuationMask fireEvacuationMask;

    [Header("Grabber")]
    public Grabber leftGrabber;

    private void OnEnable()
    {
        // Grabber 이벤트 등록
        if (leftGrabber != null)
        {
            leftGrabber.OnGrabEvent += HandkerGrab;
        }

        else
            Debug.LogError("[MaskWarningUI] leftGrabber -> null");

        // FireEvacuationMask 이벤트 등록
        if (fireEvacuationMask != null)
        {
            fireEvacuationMask.OnHandkerchiefEnter += HandkerEnter;
            fireEvacuationMask.OnHandkerchiefExit += HandkerExit;
        }
        else
            Debug.LogError("[MaskWarningUI] fireEvacuationMask -> null");
    }

    private void OnDisable()
    {
        // Grabber 이벤트 해제
        if (leftGrabber != null)
        {
            leftGrabber.OnGrabEvent -= HandkerGrab;
        }

        // FireEvacuationMask 이벤트 해제
        if (fireEvacuationMask != null)
        {
            fireEvacuationMask.OnHandkerchiefEnter -= HandkerEnter;
            fireEvacuationMask.OnHandkerchiefExit -= HandkerExit;
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandkerEnter()
    {
        // 패널 비활성화
        maskWarningPanel.SetActive(false);
        Debug.Log("손수건과 충돌할 때 실행");
    }

    // 손수건과 충돌 종료할 때 실행
    private void HandkerExit()
    {
        // 패널 활성화
        maskWarningPanel.SetActive(true);
        Debug.Log("손수건과 충돌 종료할 때 실행");
    }

    // 손수건을 잡을 때 실행
    private void HandkerGrab()
    {
        // 패널 활성화
        maskWarningPanel.SetActive(true);
        Debug.Log("손수건을 잡을 때 실행");
    }
}