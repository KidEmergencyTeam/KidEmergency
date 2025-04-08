using UnityEngine;

[CreateAssetMenu(fileName = "JSWDialog", menuName = "DialogSystem/JSWDialog")]
public class JSWDialogData : ScriptableObject
{
	public string[] dialogs;
	public AudioClip[] audios;
	public bool isAnimation;
	public string animationName;
	public int emotionEye;
	public int emotionMouth;
	public int emotionColor;
	public FEStateType nextState;
}