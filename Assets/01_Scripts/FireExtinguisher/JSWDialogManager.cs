using System;
using System.Collections;
using UnityEngine;

public class JSWDialogManager : MonoBehaviour
{
	public static JSWDialogManager Instance { get; set; }

	private void Awake()
	{
		if (Instance) Destroy(gameObject);
		else Instance = this;
	}

	public void DialogStart() // 첫 장면이 시작될 때 사용되는 메서드
	{
		StartCoroutine(ShowDialog());
	}

	public IEnumerator ShowDialog()
	{
		RobotController robot = FindObjectOfType<RobotController>();

		UIManager.Instance.dialogUI.dialogPanel.SetActive(true);

		foreach (string dialog in FEScene.Instance.currentDialog)
		{
			yield return StartCoroutine(TypingEffect(dialog));
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
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitUntil(() => UIManager.Instance.dialogUI.dialogText.text == text);
		yield return new WaitForSeconds(1f);
	}
}