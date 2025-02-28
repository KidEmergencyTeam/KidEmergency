using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "DialogSystem/Dialog")]
public class Dialog : ScriptableObject
{
    [TextArea]
    public string[] dialogs; // 관련 대사
    
    [Serializable]
    public class DialogChoice
    {
        public string optionText;        // 선택지 텍스트
        public string highlightText;     // 강조할 텍스트
        public Sprite optionSprite;      // 선택지 이미지
        public Dialog nextOptionDialog;    // 선택 시 다음 대화
        public ActionType choiceOptionNextActionType;    // 선택 시 실행할 액션 타입
    }
    
    public DialogChoice[] choices; // 대화 후 선택지들
    public string nextScene; // 로드할 씬 이름
    public Dialog nextDialog; // 선택지 없을 때 다음 대화
    public ActionType nextActionType; // 선택지 없을 때 실행할 다음 액션
}