using UnityEngine;

// 현재 경고창 상태를 저장하는 용도
public class MaskWarningUIStateManager : DisableableSingleton<MaskWarningUIStateManager>
{
    // 내부 변수에 충돌 상태를 저장하기 위한 개념
    private bool isCollided = true;

    // 충돌 상태 저장하기
    public void SetCollisionState(bool setState)
    {
        // 내부 변수에 충돌 상태 저장
        isCollided = setState;
        Debug.Log("충돌 상태 저장 완료");
    }

    // 충돌 상태 불러오기
    public bool GetCollisionState()
    {
        // 내부 변수에 저장된 충돌 상태 불러오기
        bool currentState = isCollided;

        // 내부 변수에 저장된 충돌 상태 반환
        return currentState;
    }
}
