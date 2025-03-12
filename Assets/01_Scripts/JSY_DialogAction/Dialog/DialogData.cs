using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialog", menuName = "DialogSystem/Dialog")]
public class DialogData : ScriptableObject
{
    [TextArea]
    [Tooltip("대사")]
    public string[] dialogs;
    
    [Serializable]
    public class DialogChoice
    {
        [Tooltip("선택지 텍스트 설정")]
        public string optionText;
        [Tooltip("강조할 텍스트 설정")]
        public string highlightText; 
        [Tooltip("선택지 이미지 설정")]
        public Sprite optionSprite; 
        [Tooltip("선택 후 다음 대화(Dialog Data) 설정")]
        public DialogData nextOptionDialog; 
        [Tooltip("선택 후 실행할 액션(Action Type) 설정")]
        public ActionType choiceOptionNextActionType;
        [Tooltip("이 선택지가 정답인지 여부")]
        public bool isAnswer;
    }
    
    [Tooltip("대화가 끝난 후 실행될 선택지 설정(없으면 비워두세요)")]
    public DialogChoice[] choices;
    [Tooltip("대화가 끝난 후 강조될 오브젝트들(없으면 비워두세요)")]
    public GameObject[] objects;
    [Tooltip("대화가 끝난 후 로드할 씬 이름 설정(없으면 비워두세요)")]
    public string nextScene;
    [Tooltip("대화가 끝난 후 변경될 시점값(없으면 비워두세요)")]
    public float viewValue;
    [Tooltip("대화가 끝난 후 실행할 다음 대화(Dialog Data) 설정(선택지가 있으면 비워두세요)")]
    public DialogData nextDialog;
    [Tooltip("대화가 끝난 후 실행할 다음 액션(Action Type) 설정(선택지가 있으면 비워두세요)")]
    public ActionType nextActionType;
}