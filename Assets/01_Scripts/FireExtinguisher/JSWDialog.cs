using UnityEngine;

[CreateAssetMenu(fileName = "JSWDialog", menuName = "DialogSystem/JSWDialog")]
public class JSWDialog : ScriptableObject
{
	public string[] dialogs;
	public string emotion;
	public FEState nextState;
}