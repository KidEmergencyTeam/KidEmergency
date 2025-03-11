using UnityEngine;

// 컨트롤러의 레이가 버튼을 향했을 때,
// 처리되는 값을
// 좌측으로 처리할 건지 우측으로 처리할 건지 구분해주는 스크립트
// 좋은 방법은 아니라고 생각

public class PlayerPointerId : MonoBehaviour
{
    // 좌측 포인터 ID 열거형
    public enum LeftPointerId
    {
        None = -1,
        LeftPointer0,
        LeftPointer1,
        LeftPointer2,
        LeftPointer3,
        LeftPointer4,
        LeftPointer5,
        LeftPointer6,
        LeftPointer7,
        LeftPointer8,
        LeftPointer9,
        LeftPointer10,

        // 필요한 만큼 추가 가능
    }

    // 우측 포인터 ID 열거형
    public enum RightPointerId
    {
        None = -1,
        RightPointer0,
        RightPointer1,
        RightPointer2,
        RightPointer3,
        RightPointer4,
        RightPointer5,
        RightPointer6,
        RightPointer7,
        RightPointer8,
        RightPointer9,
        RightPointer10,

        // 필요한 만큼 추가 가능
    }

    [Header("좌측 포인터 ID")]
    public LeftPointerId[] leftPointerIds = new LeftPointerId[] { LeftPointerId.LeftPointer0 };

    [Header("우측 포인터 ID")]
    public RightPointerId[] rightPointerIds = new RightPointerId[] { RightPointerId.RightPointer0 };
}
