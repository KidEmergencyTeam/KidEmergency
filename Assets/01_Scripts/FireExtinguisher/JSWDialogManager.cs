using System.Collections;
using UnityEngine;

public class JSWDialogManager : MonoBehaviour
{
	public static JSWDialogManager Instance { get; set; }
	public float delay = 1f;

	private void Awake()
	{
		if (Instance) Destroy(gameObject);
		else Instance = this;
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

		for (int i = 0; i < FEScene.Instance.currentDialog.Length; i++)
		{
			audio.clip = FEScene.Instance.currentDialogData.audios[i];
			audio.Play();
			yield return StartCoroutine(TypingEffect(FEScene.Instance.currentDialog[i]));
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