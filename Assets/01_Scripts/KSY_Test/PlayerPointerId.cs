using UnityEngine;

// 각 플레이어별 포인터 ID 관리 및 사용자 구분
public class PlayerPointerId : MonoBehaviour
{
    // 플레이어 구분을 위해 사용
    [Header("플레이어 ID")]
    public int userId; 

    // 포인터 ID를 토대로 
    // 해당 값이 좌측인지 우측인지를 
    // 명시적으로 구분
    // 예) 레이가 버튼위에 있을때 출력 되는 값이 1 -> 좌측으로 명시 또는 우측으로 명시

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
