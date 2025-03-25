public enum ActionType
{
    Basic, // 아무 것도 안 하는 기본 액션
    ShowDialog, // 대화창 활성화
    ShowOption, // 옵션 활성화
    ChangeScene, // 씬 전환
    Earthquake, // 지진 시작
    ChangeView, // 시점 변경
    HighlightObject, // 오브젝트 강조
    HoldingLeg, // 책상 다리 잡기
    PlaceObject, // 오브젝트 배치
    FixingBag, // 가방 고정
    EndGame // 게임 끝. 타이틀로 돌아가기
}
