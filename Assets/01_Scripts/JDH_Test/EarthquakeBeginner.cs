using EPOOutline;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EarthquakeBeginner : MonoBehaviour
{
	public enum PLACE
	{
		CLASSROOM,
		HALLWAY,
		STAIRS_ELEVATOR,
		OUTSIDE,
		HOUSE,
	};

	[Header("���� ���")] public PLACE place;

	[Header("���� ��Ȳ ����")] public bool isEarthquakeStart;

	[Header("NPC �� �÷��̾� �̵� ���� ����")]
	public GameObject player; // �÷��̾��� ���� ��ġ

	public GameObject seti;
	public GameObject[] NPC; // ��Ÿ NPC���� ���� ��ġ
	public FadeInOut fadeInOutImg;
	public TestButton2 okBtn;
	public GameObject warningUi;
	public GameObject exampleDescUi;
	public GameObject leftHand; // �޼� ���� ������Ʈ
	public Canvas playerUi; //�÷��̾� UI Canvas

	[Header("����, Ż�ⱸ, ��� ������Ʈ")] [SerializeField]
	private GameObject backpack;

	[SerializeField]
	private GameObject
		emergencyExit; // ��� ������Ʈ (Outlinable ������Ʈ�� �߰��� ���)

	[SerializeField] private GameObject fireAlarm;
	// public EarthquakeSystem earthquake;

	[Header("�Ӹ� ��ġ üũ")] public bool isHeadDown = false;

	public float
		headHeightThreshold; // �Ӹ� ���� ���� (�� ������ ������ �Ӹ��� ���� ������ �Ǵ�)

	[SerializeField] private Transform xrCamera; // HMD ī�޶�
	[SerializeField] private float initialHeight; // �ʱ� �÷��̾� ����

	[Header("���� ���� ��ġ")] [SerializeField]
	private Transform playerSpawnPos;

	[SerializeField] private Transform[] npcSpawnPos;
	[Header("�̵� ��ǥ ��ġ")] public Transform playerMovPos;
	public Transform setiMovPos;
	public Transform[] npcMovPos;

	[Header("���� UI �̹���")] [SerializeField]
	private Image LeftImg;

	[SerializeField] private Image RightImg;
	[SerializeField] private TestButton2 LeftBtn;
	[SerializeField] private TestButton2 RightBtn;
	[SerializeField] private Sprite leftChangeImg;
	[SerializeField] private Sprite rightChangeImg;
	[SerializeField] private TextMeshProUGUI descriptionText;

	[Header("��Ȳ ���� üũ")] public bool isFirstStepRdy;
	public bool isSecondStepRdy;
	public bool hasHandkerchief;
	public bool ruleCheck;

	[Header("��ȭ �ý���")] [SerializeField]
	private BeginnerDialogSystem firstDialog;

	[SerializeField] private BeginnerDialogSystem secondDialog;
	[SerializeField] private BeginnerDialogSystem thirdDialog;
	[SerializeField] private BeginnerDialogSystem forthDialog;
	[SerializeField] private BeginnerDialogSystem fifthDialog;
	[SerializeField] private BeginnerDialogSystem sixthDialog;
	[SerializeField] private BeginnerDialogSystem seventhDialog;
	[SerializeField] private BeginnerDialogSystem leftChoiceDialog;
	[SerializeField] private BeginnerDialogSystem rightChoiceDialog;

	private void Awake()
	{
		xrCamera = Camera.main.transform;
		initialHeight = xrCamera.position.y; // �ʱ� �÷��̾� ���� ����
		// earthquake = FindObjectOfType<EarthquakeSystem>();
	}

	// ���� ���� �� ����
	IEnumerator Start()
	{
		switch (place)
		{
			// ����
			case PLACE.CLASSROOM:
				// 1. ù ��° ��ȭ ����
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
				//2. ȭ���� ���ݾ� ��鸮�� �溸���� �︮�� ������ ���� ��ü�� �Ѿ����� ���� �Ҹ��� ���۵�
				isEarthquakeStart = true;
				//���� ����
				// earthquake.StartEarthquake();
				fireAlarm.gameObject.SetActive(true);
				// 3. �� ��° ��ȭ ����
				secondDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
				// 4. å�� ������ ���� ������ UI �߻� UI�� ������ å�� ������ ���� ��ȯ ���� ��� ����
				okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "å�� ������";
				okBtn.gameObject.SetActive(true);
				yield return new WaitUntil(() => okBtn.isClick == true);
				okBtn.gameObject.SetActive(false);
				okBtn.isClick = false;
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				//�÷��̾�� NPC�� ��ǥ �̵� �� �ൿ ��ȭ
				TeleportCharacters();
				SetAllNpcState(NpcRig.State.DownDesk);
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == true);
				thirdDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);

				// 5. ���� �տ��ִ� å�� �ٸ��� �ƿ������� Ȱ��ȭ ���� ��� ���
				//�ƿ����� Ȱ��ȭ
				forthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => forthDialog.isDialogsEnd == true);
				SetAllNpcState(NpcRig.State.DownDesk); //NPC �ൿ ����
				//å�� �ٸ� ���

				// 6. å�� ���� ������ �ִ� ������ �׵θ��� �����ϸ� ���̵鿡�� ��ġ�� Ȯ�ν�Ų��.
				//���� ������Ʈ �ƿ����� Ȱ��ȭ
				backpack.GetComponent<Outlinable>().enabled = true;
				fifthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true);
				// 7. ��鸲�� ���� ���� ��� ���
				//��鸲 ����
				// earthquake.StopEarthquake(); // ���� ����
				// yield return new WaitUntil(() => earthquake._endEarthquake == true);

				sixthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);
				//å�� ������ ��ư Ȱ��ȭ �� �ؽ�Ʈ ����
				okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "å�� ������";
				okBtn.gameObject.SetActive(true);
				yield return new WaitUntil(() => okBtn.isClick == true);
				okBtn.gameObject.SetActive(false);
				//���� ��ġ�� �̵� �� NPC ��� ����
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				ReturnToOriginalPosition(); //���� ��ġ�� �̵�
				SetAllNpcState(NpcRig.State.None);
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				// 8. å�� ���� ������ �ִ� ������ �׵θ��� ������Ű�� ���̵鿡�� �����ϰ� �Ѵ�. ���� ���� ��� ���
				seventhDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => seventhDialog.isDialogsEnd == true);
				//������ ������ �տ� �����ȴ�. �Ӹ� ��ġ���� �ø��� ������ ��� ��� �� fadeout���� Scene �̵�, ������ ���� �� NPC ���� ����

				//9. Scene�̵�
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("JDH_Earth2");
				break;

			case PLACE.HALLWAY:
				SetAllNpcState((NpcRig.State.HoldBag));
				//��� ��� �� ���Scene���� �̵�
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
				//�ǳ� ������ ���� ��Ʈ�ѷ��� select�ϸ� outliner Ȱ��ȭ
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("JDH_Earth3");
				break;

			case PLACE.STAIRS_ELEVATOR:
				SetAllNpcState((NpcRig.State.HoldBag));
				// 1. ù ��° ��ȭ ����
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
				//�̸� ������ �̹��� ����
				LeftImg.sprite = leftChangeImg;
				RightImg.sprite = rightChangeImg;
				isFirstStepRdy = true;
				yield return new WaitUntil(() => isFirstStepRdy == true);

				// 2.��ܰ� ���������� �������� �������� ������, ��ư ���� �� ��� ����
				exampleDescUi.gameObject.SetActive(true);
				//
				LeftBtn.GetComponent<Button>().onClick
					.AddListener(() => HandleChoice(true));
				RightBtn.GetComponent<Button>().onClick
					.AddListener(() => HandleChoice(false));
				yield return new WaitUntil(() =>
					leftChoiceDialog.isDialogsEnd == true ||
					rightChoiceDialog.isDialogsEnd == true);

				//3.�� ��° ��ȭ ���� ����
				thirdDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
				//Scene�̵�
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("JDH_Earth4");
				break;
			case PLACE.OUTSIDE:
				// 1. ù ��° ��ȭ ����
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

				//Scene�̵�
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene(0); //Ÿ��Ʋ�� �̵�
				break;
		}
	}

	private void Update()
	{
		DetectHeadLowering(); // �Ӹ� ���� ����
	}
	//�̵� �� Ui��ġ ����

	public void AdjustUiTransform()
	{
		if (playerUi == null || player == null) return;

		RectTransform rectTransform = playerUi.GetComponent<RectTransform>();
		if (rectTransform == null) return;

		// ���� ���� ��ġ ����
		Vector3 worldPosition = player.transform.position +
		                        player.transform.TransformDirection(
			                        new Vector3(0.75f, 1.5f, 0.5f));
		rectTransform.position = worldPosition;

		// ���� ȸ�� ���� (�־��� �� �״�� ����)
		rectTransform.localRotation =
			Quaternion.Euler(-20, 50, 0); // ���� ������ �ƴ�, �״�� ����

		Debug.Log("playerUi ��ġ �� ȸ�� ���� �Ϸ� (Local Space)");
	}

	private void TeleportCharacters()
	{
		// �÷��̾� �̵�
		if (playerMovPos != null)
		{
			player.transform.position = playerMovPos.position;
			player.transform.rotation = playerMovPos.rotation;
		}

		// ��Ƽ �̵�
		if (setiMovPos != null)
		{
			seti.transform.position = setiMovPos.position;
		}

		// NPC �̵�
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcMovPos.Length > i && npcMovPos[i] != null)
			{
				NPC[i].transform.position = npcMovPos[i].position;
			}
		}

		Debug.Log("�÷��̾� �� NPC �̵� �Ϸ�");
	}

	// ���� ��ġ�� �ǵ����� �Լ�
	public void ReturnToOriginalPosition()
	{
		// �÷��̾� ��ġ ����
		if (playerSpawnPos != null)
		{
			player.transform.position = playerSpawnPos.position;
			player.transform.rotation = playerSpawnPos.rotation;
		}

		// NPC ��ġ ����
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcSpawnPos.Length > i && npcSpawnPos[i] != null)
			{
				NPC[i].transform.position = npcSpawnPos[i].position;
				NPC[i].transform.rotation = npcSpawnPos[i].rotation;
			}
		}

		Debug.Log("�÷��̾� �� NPC�� ���� ��ġ�� �����߽��ϴ�.");
	}

	// �θ� ������Ʈ�� ��� �ڽĿ��� Outlinable ������Ʈ Ȱ��ȭ
	private void ActiveOutlineToChildren(GameObject parent)
	{
		if (parent == null) return;

		foreach (Transform child in parent.GetComponentsInChildren<Transform>())
		{
			Outlinable outline = child.gameObject.GetComponent<Outlinable>();

			if (outline == null)
			{
				outline = child.gameObject.AddComponent<Outlinable>();
			}

			outline.enabled = true; // Outlinable Ȱ��ȭ
		}
	}

	// �Ӹ� ���� ���� �Լ�
	private void DetectHeadLowering()
	{
		if (xrCamera == null) return;

		float currentHeight = xrCamera.position.y; // ���� �Ӹ� ����

		if (currentHeight < headHeightThreshold)
		{
			isHeadDown = true;
			// Debug.Log("�Ӹ��� �������ϴ�! (Y �� ����)");
		}
		else
		{
			isHeadDown = false;
		}
	}

	// ��� NPC���� ���¸� �����ϰ� �����ϴ� �Լ�
	public void SetAllNpcState(NpcRig.State newState)
	{
		foreach (GameObject npc in NPC)
		{
			if (npc != null) // NPC�� null�� �ƴ��� Ȯ��
			{
				npc.GetComponent<NpcRig>().state = newState; // NPC�� ���¸� ����
			}
		}
	}

	void HandleChoice(bool isLeftChoice)
	{
		exampleDescUi.SetActive(false);

		if (isLeftChoice)
		{
			leftChoiceDialog.gameObject.SetActive(true);
			Debug.Log("���� ������ ��� ���");
		}
		else
		{
			rightChoiceDialog.gameObject.SetActive(true);
			Debug.Log("������ ������ ��� ���");
		}
	}
}