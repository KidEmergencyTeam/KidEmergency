using UnityEngine;

[CreateAssetMenu(fileName = "JSWDialog", menuName = "DialogSystem/JSWDialog")]
public class JSWDialog : ScriptableObject
{
	public string[] dialogs;
	public AudioClip[] audios;
	public string emotion;
	public FEStateType nextState;
}