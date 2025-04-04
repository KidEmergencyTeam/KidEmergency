using UnityEngine;

// Grabbable 스크립트를 할당
// public bool isGrabbable = false;면 -> 경고창 비활성화
// 메서드를 만들어서 -> Start에서 처리

public class MaskWarningUI : MonoBehaviour
{
    [Header("경고창")]
    public GameObject maskWarningPanel;

    [Header("FireEvacuationMask")]
    public FireEvacuationMask fireEvacuationMask;

    [Header("Grabber")]
    public Grabber leftGrabber;

    private void Start()
    {
        // 기본적으로 패널은 비활성화
        if (maskWarningPanel != null)
        {
            maskWarningPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("maskWarningPanel -> null");
        }

        //// Grabber 이벤트에 핸들러 등록
        if (leftGrabber != null)
        {
            // 손수건을 잡을 때 HandkerGrab 실행
            leftGrabber.OnGrabEvent += HandkerGrab;

            // 손수건을 놓을 때 HandkerRelease 실행
            //leftGrabber.OnReleaseEvent += HandkerRelease;
        }
        else
        {
            Debug.LogError("grabber.cs -> null");
        }

        // FireEvacuationMask 이벤트에 핸들러 등록
        if (fireEvacuationMask != null)
        {
            // 손수건과 충돌할 때 HandkerEnter 실행
            fireEvacuationMask.OnHandkerchiefEnter += HandkerEnter;

            // 손수건과 충돌 종료할 때 HandkerExit 실행
            fireEvacuationMask.OnHandkerchiefExit += HandkerExit;
        }
        else
        {
            Debug.LogError("FireEvacuationMask.cs -> null");
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandkerEnter() // 의존
    {
        // 패널 비활성화
        maskWarningPanel.SetActive(false);
        Debug.Log("손수건과 충돌할 때 실행");
    }

    // 손수건과 충돌 종료할 때 실행
    private void HandkerExit() // 의존
    {
        // 패널 활성화
        maskWarningPanel.SetActive(true);
        Debug.Log("손수건과 충돌 종료할 때 실행");
    }

    // 손수건을 잡을 때 실행 -> 호출은 한번만 합니다. 한번 잡을때 호출하고 나면
    private void HandkerGrab()
    {
        // 패널 활성화
        maskWarningPanel.SetActive(true);
        Debug.Log("손수건을 잡을 때 실행");
    }

    // 손수건을 놓을 때 실행
    private void HandkerRelease()
    {
        // 패널 비활성화
        maskWarningPanel.SetActive(false);
        Debug.Log("손수건을 놓을 때 실행");
    }
}