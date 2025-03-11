using UnityEngine;

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
