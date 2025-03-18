using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JSWLobbyManager : MonoBehaviour
{
	public GameObject titleMenu;
	public Button enterMultiKindergarten;
	public Button enterSingleKindergarten;
	public Button enterMultiSchool;
	public Button enterSingleSchool;
	public Button enterMultiPark;
	public Button enterSinglePark;

	private void Start()
	{
		titleMenu.SetActive(true);
		enterMultiKindergarten.onClick.AddListener(EnterMultiKindergarten);
		enterSingleKindergarten.onClick.AddListener(EnterSingleKindergarten);
		enterMultiSchool.onClick.AddListener(EnterMultiSchool);
		enterSingleSchool.onClick.AddListener(EnterSingleSchool);
		enterMultiPark.onClick.AddListener(EnterMultiPark);
		enterSinglePark.onClick.AddListener(EnterSinglePark);
	}

	private void EnterMultiKindergarten()
	{
		titleMenu.SetActive(false);
		SceneManager.LoadScene("JSW 1-1");
	}

	private void EnterSingleKindergarten()
	{
	}

	private void EnterMultiSchool()
	{
	}

	private void EnterSingleSchool()
	{
	}

	private void EnterMultiPark()
	{
	}

	private void EnterSinglePark()
	{
	}
}