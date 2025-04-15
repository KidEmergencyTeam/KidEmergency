using UnityEngine;

// 경고창 ui 관리 스크립트
public class MaskWarningUI : MonoBehaviour
{
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

    private void Start()
    {
        if (MaskWarningUIStateManager.Instance != null)
        {
            // 충돌 상태 불러오기
            bool currentState = MaskWarningUIStateManager.Instance.GetCollisionState();
            Debug.Log("충돌 상태 불러오기 완료");

            // 충돌 상태라면 -> true를 불러오면
            if (currentState)
            {
                // 경고창 비활성화
                UIManager.Instance.CloseWarningUI();

                // 메시지 창 활성화
                //UIManager.Instance.dialogUI.gameObject.SetActive(true);
                Debug.Log("경고창 비활성화");
            }

            // 충돌 종료 상태라면 -> false를 불러오면
            else
            {
                // 경고창 활성화
                UIManager.Instance.OpenWarningUI();

                // 메시지 창 활성화
                //UIManager.Instance.dialogUI.gameObject.SetActive(false);
                Debug.Log("경고창 활성화");
            }
        }
        else
        {
            Debug.LogError("MaskWarningUIStateManager -> null");
        }
    }

    // 손수건과 충돌할 때 실행
    private void HandkerEnter()
    {
        // 경고창 비활성화
        UIManager.Instance.CloseWarningUI();

        // 메시지 창 활성화
        //UIManager.Instance.dialogUI.gameObject.SetActive(true);
        Debug.Log("손수건과 충돌할 때 실행");

        // 충돌 상태 저장하기 -> true: 충돌o
        MaskWarningUIStateManager.Instance.SetCollisionState(true);
    }

    // 손수건과 충돌 종료할 때 실행
    private void HandkerExit()
    {
        // 경고창 활성화
        UIManager.Instance.OpenWarningUI();

        // 메시지 창 비활성화
        //UIManager.Instance.dialogUI.gameObject.SetActive(false);
        Debug.Log("손수건과 충돌 종료할 때 실행");

        // 충돌 상태 저장하기 -> flase: 충돌x
        MaskWarningUIStateManager.Instance.SetCollisionState(false);
    }

    // 손수건을 잡을 때 실행
    private void HandkerGrab()
    {
        // 경고창 활성화
        UIManager.Instance.OpenWarningUI();

        // 메시지 창 비활성화
        //UIManager.Instance.dialogUI.gameObject.SetActive(false);
        Debug.Log("손수건을 잡을 때 실행");
    }
}