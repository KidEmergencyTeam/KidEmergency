using System.Collections;
using UnityEngine;

public class JSWDialogManager : MonoBehaviour
{
	public static JSWDialogManager Instance { get; set; }
	public float delay = 1f;
	public float startDelay = 1f;
	private FEScene _scene;

	private void Awake()
	{
		if (Instance) Destroy(gameObject);
		else Instance = this;
		_scene = FEScene.Instance;
	}

	public void DialogStart() // 첫 장면이 시작될 때 사용되는 메서드
	{
		StopAllCoroutines();
		StartCoroutine(ShowDialog());
	}

	public IEnumerator ShowDialog()
	{
		RobotController robot = FindObjectOfType<RobotController>();
		AudioSource audio = robot.GetComponent<AudioSource>();
		
		UIManager.Instance.dialogUI.dialogPanel.SetActive(true);
		
		yield return new WaitForSeconds(startDelay);
		
		robot.emotionChanger.SetEmotionEyes(_scene.currentDialogData.emotionEye);
		robot.emotionChanger.SetEmotionMouth(_scene.currentDialogData.emotionMouth);
		robot.robotColorManager.ChangeBodyColor(_scene.currentDialogData.emotionColor);
		if(_scene.currentDialogData.isAnimation) robot.SetAnimaiton(_scene.currentDialogData.animationName);

		for (int i = 0; i < _scene.currentDialog.Length; i++)
		{
			audio.clip = _scene.currentDialogData.audios[i];
			audio.Play();
			yield return StartCoroutine(TypingEffect(_scene.currentDialog[i]));
		}

		UIManager.Instance.dialogUI.dialogPanel.SetActive(false);
		robot.SetBasic();
	}

	private IEnumerator TypingEffect(string text)
	{
		UIManager.Instance.dialogUI.dialogText.text = "";

		foreach (char letter in text.ToCharArray())
		{
			UIManager.Instance.dialogUI.dialogText.text += letter;
			yield return new WaitForSeconds(0.16f);
		}

		yield return new WaitUntil(() => UIManager.Instance.dialogUI.dialogText.text == text);
		yield return new WaitForSeconds(delay);
	}
}