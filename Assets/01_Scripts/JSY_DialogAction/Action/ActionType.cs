public enum ActionType
{
    Basic, // 아무 것도 안 하는 기본 액션
    ShowDialog, // 대화창 활성화
    ShowOption, // 옵션 활성화
    ChangeScene, // 씬 전환
    Earthquake, // 지진 시작
    ChangeView, // 시점 변경
    OutlineObject, // 오브젝트 아웃라인 강조
    HoldLeg, // 책상 다리 잡기
    PlaceObject, // 오브젝트 배치
    FixBag, // 가방 고정
    CloseGasValve, // 가스밸브 잠그기
    OpenCircuitBox, // 전기 차단기 열기
    LowerCircuitLever, // 차단기 레버 내리기
    SelectGuideLine, // 유도선 선택
    EndGame // 게임 끝. 타이틀로 돌아가기
}
