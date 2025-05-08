using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EarthquakeBeginner : MonoBehaviour
{
	public enum PLACE
	{
		CLASSROOM, // 교실
		HALLWAY, // 복도
		STAIRS_ELEVATOR, // 계단/엘리베이터
		OUTSIDE, // 건물 밖
		HOUSE // 집
	};

	[Header("현재 위치")] public PLACE place;

	[Header("지진 시작 여부")] public bool isEarthquakeStart;

<<<<<<< HEAD
    [Header("NPC 및 플레이어 이동 관련 오브젝트")]
    public GameObject player; // 플레이어 캐릭터 오브젝트
    public RobotController seti;
    public GameObject[] NPC; // NPC 캐릭터 오브젝트 배열
    public FadeInOut fadeInOutImg;
    public Button okBtn;
    public GameObject warningUi;
    public GameObject exampleDescUi;
    public GameObject leftHand; // 메뉴 조작 핸드 오브젝트
    public Canvas playerUi; // 플레이어 UI Canvas
    public GameObject exitLineUI;
    public GameObject grabDeskLegUI;
    public GameObject grabBagUI;
=======
	[Header("NPC 및 플레이어 이동 관련 오브젝트")] public GameObject player; // 플레이어 캐릭터 오브젝트
	public RobotController seti;
	public GameObject[] NPC; // NPC 캐릭터 오브젝트 배열
	public FadeInOut fadeInOutImg;
	public Button okBtn;
	public GameObject warningUi;
	public GameObject exampleDescUi;
	public GameObject leftHand; // 메뉴 조작 핸드 오브젝트
	public Canvas playerUi; // 플레이어 UI Canvas
	public GameObject exitLineUI;
	public GameObject grabDeskLegUI;
>>>>>>> b1d4965c03f4cbdf3147c17c8f36fed144c95689

	[Header("가방, 출구, 경보 장치 오브젝트")] [SerializeField]
	private GameObject backpack;

	[SerializeField]
	private GameObject emergencyExit; // 비상 출구 (Outlinable 컴포넌트 추가 필요)

	[SerializeField] private GameObject fireAlarm;
	[SerializeField] private GameObject emergencyDisaterMessage;
	[SerializeField] private GameObject earthquakeSound;
	[SerializeField] private ExitLine advEmergencyExitLine;
	public EarthquakeSystem earthquake;

	[Header("고개 숙임 여부 체크")] public bool isHeadDown = false;
	public float headHeightThreshold; // 고개 숙임을 판단하는 높이 기준
	[SerializeField] private Transform xrCamera; // HMD 카메라
	[SerializeField] private float initialHeight; // 초기 플레이어 높이

	[Header("플레이어 및 NPC 스폰 위치")] [SerializeField]
	private Transform playerSpawnPos;

	[SerializeField] private Transform[] npcSpawnPos;

	[Header("이동 목표 위치")] public Transform playerMovPos;
	public Transform setiMovPos;
	public Transform[] npcMovPos;

	[Header("선택 UI 이미지 및 버튼")] [SerializeField]
	private Image LeftImg;

	[SerializeField] private Image RightImg;
	[SerializeField] private Button LeftBtn;
	[SerializeField] private Button RightBtn;
	[SerializeField] private Sprite leftChangeImg;
	[SerializeField] private Sprite rightChangeImg;
	[SerializeField] private Image warningImg;
	[SerializeField] private Sprite protectedHeadImg;
	[SerializeField] private Sprite grabDeskLegImg;

	[Header("진행 상태 체크 변수")] public bool isFirstStepRdy;
	public bool isSecondStepRdy;
	public bool hasHandkerchief;
	public bool ruleCheck;
	public bool doProtectedHead; //머리를 보호하고 있어야 하는 구간
	public bool isprotectedHead; //머리를 잘 보호하는지 확인하는 변수
	public bool isButtonClick;
	public bool doGrapDeskLeg;
	public GameObject deskLegObj; //책상 다리 잡기 오브젝트
	public GameObject bagObj; //가방 오브젝트
	public GameObject grapDeskCheck;

	[Header("대화 시스템")] [SerializeField] private BeginnerDialogSystem firstDialog;
	[SerializeField] private BeginnerDialogSystem secondDialog;
	[SerializeField] private BeginnerDialogSystem thirdDialog;
	[SerializeField] private BeginnerDialogSystem forthDialog;
	[SerializeField] private BeginnerDialogSystem fifthDialog;
	[SerializeField] private BeginnerDialogSystem sixthDialog;
	[SerializeField] private BeginnerDialogSystem seventhDialog;
	[SerializeField] private BeginnerDialogSystem eighthDialog;
	[SerializeField] private BeginnerDialogSystem leftChoiceDialog;
	[SerializeField] private BeginnerDialogSystem rightChoiceDialog;

	private void Awake()
	{
		xrCamera = Camera.main.transform;
		initialHeight = xrCamera.position.y; // 초기 플레이어 높이 설정
		earthquake = FindObjectOfType<EarthquakeSystem>();
	}

	IEnumerator Start()
	{
		switch (place)
		{
			// 교실
			case PLACE.CLASSROOM:
				// 1. 첫 번째 대화 시작
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

<<<<<<< HEAD
                //가방을 주워야 하는 UI 출력
                grabBagUI.SetActive(true);
                yield return new WaitUntil(() => leftHand.GetComponent<Grabber>().currentGrabbedObject != null);
                grabBagUI.SetActive(false);

                sixthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);
=======
				// 2. 화면이 흔들리며 지진 발생. 화재 경보도 함께 울림.
				isEarthquakeStart = true;
				seti.SetLookingFor();
				earthquake.StartEarthquake();
				fireAlarm.gameObject.SetActive(true);
				emergencyDisaterMessage.SetActive(true);
				earthquakeSound.gameObject.SetActive(true);
				// 3. 두 번째 대화 시작
				secondDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
				seti.SetBasic();
				// 4. 책상 밑으로 들어가라는 메시지를 띄우는 UI 활성화
				okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "책상 밑으로";
				okBtn.GetComponent<Button>().onClick.AddListener(() => OkBtnClick());
				okBtn.gameObject.SetActive(true);
				yield return new WaitUntil(() => isButtonClick == true);
				okBtn.gameObject.SetActive(false);
				isButtonClick = false;
>>>>>>> b1d4965c03f4cbdf3147c17c8f36fed144c95689

				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);

				// 플레이어와 NPC 이동 및 행동 변경 후 대사 출력
				TeleportCharacters();
				SetAllNpcState(NpcRig.State.DownDesk);
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);

				//5. 책상 밑에서 대사가 완료된 후 책상 다리를 잡도록 Outline활성화 및 잡기

				thirdDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
				doGrapDeskLeg = true;
				//책상 다리 outline 활성화
				//grabDeskLegUI.SetActive(true);
				deskLegObj.GetComponent<DeskLeg>().enabled = true;
				//책상 다리를 잡을때까지 대기 ->5초
				yield return new WaitUntil(() =>
					deskLegObj.GetComponent<DeskLeg>().isHoldComplete == true);
				doGrapDeskLeg = false;
				deskLegObj.GetComponent<DeskLeg>().enabled = false;

				// DeskLegObj의 모든 자식 게임 오브젝트 비활성화
				foreach (Transform child in deskLegObj.transform)
				{
					child.gameObject.SetActive(false);
				}
				//grabDeskLegUI.SetActive(false);

				forthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => forthDialog.isDialogsEnd == true);
				// 6.가방을 찾을 수 있도록 유도하는 이벤트 발생
				bagObj.GetComponent<Bag>().enabled = true;
				//bagObj.GetComponentInChildren<GameObject>().SetActive(true);
				fifthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true);
				// 7. 지진 종료
				earthquake.StopEarthquake();
				earthquakeSound.gameObject.SetActive(false);
				yield return new WaitUntil(() => earthquake._endEarthquake == true);

				sixthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);

				// 플레이어와 NPC를 원래 위치로 이동
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				ReturnToOriginalPosition();
				SetAllNpcState(NpcRig.State.None);
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);

				//가방을 줍도록 하는 대사 출력
				ruleCheck = true;
				seventhDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => seventhDialog.isDialogsEnd == true);
				//머리 보호구간
				ruleCheck = true;
				doProtectedHead = true;
				//가방을 주운뒤 머리 위에 올릴 때 까지 대기
				yield return new WaitUntil(() => isprotectedHead == true);
				//8. 마지막 대사가 끝난 후 복도로 이동
				eighthDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() =>
					eighthDialog.isDialogsEnd == true && isprotectedHead == true);
				//다음 씬으로 이동
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("Eq_Kinder_2");
				break;

			// 복도
			case PLACE.HALLWAY:
				//머리 보호구간
				ruleCheck = true;
				doProtectedHead = true;
				SetAllNpcState(NpcRig.State.HoldBag);

				// 첫 번째 대화 후 씬 이동
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

				//유도선 선택 대기 후 선택 시 모든 자식오브젝트의 outliner활성화
				advEmergencyExitLine.ExitLineInteraction();
				exitLineUI.SetActive(true);
				// 선택 완료까지 대기
				yield return new WaitUntil(() =>
					advEmergencyExitLine.isSelected == true && isprotectedHead == true);
				Debug.Log("비상구 유도선 선택 완료!");
				exitLineUI.SetActive(false);
				// 씬 전환
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("Eq_Kinder_3");
				break;

			// 계단과 엘리베이터
			case PLACE.STAIRS_ELEVATOR:
				//머리 보호구간
				ruleCheck = true;
				doProtectedHead = true;
				SetAllNpcState(NpcRig.State.HoldBag);

				// 1. 첫 번째 대화 시작
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() =>
					isprotectedHead == true && firstDialog.isDialogsEnd == true);

				// 2. 선택지 이미지 변경 및 버튼 활성화
				LeftImg.sprite = leftChangeImg;
				RightImg.sprite = rightChangeImg;
				isFirstStepRdy = true;
				yield return new WaitUntil(() => isFirstStepRdy == true);

				// 3. 선택지 UI 표시
				exampleDescUi.gameObject.SetActive(true);
				LeftBtn.GetComponent<Button>().onClick
					.AddListener(() => HandleChoice(true));
				RightBtn.GetComponent<Button>().onClick
					.AddListener(() => HandleChoice(false));

				yield return new WaitUntil(() =>
					(leftChoiceDialog.isDialogsEnd == true ||
					 rightChoiceDialog.isDialogsEnd == true)
					&& isprotectedHead == true);

				seti.SetBasic();

				// 4. 다음 대화 진행
				thirdDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() =>
					thirdDialog.isDialogsEnd == true && isprotectedHead == true);

				// 씬 전환
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene("Eq_Kinder_4");
				break;

			// 건물 밖
			case PLACE.OUTSIDE:
				seti.SetHappy();
				// 1. 첫 번째 대화 시작
				StartCoroutine(FadeInOut.Instance.FadeIn());
				yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
				firstDialog.gameObject.SetActive(true);
				yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
				seti.SetBasic();
				// 씬 전환 (타이틀 화면으로 이동)
				StartCoroutine(FadeInOut.Instance.FadeOut());
				yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
				SceneManager.LoadScene(0);
				break;
		}
	}


	private void Update()
	{
		if (ruleCheck == true && doProtectedHead == true &&
		    isprotectedHead == false)
		{
			warningImg.sprite = protectedHeadImg;
			warningUi.GetComponentInChildren<TextMeshProUGUI>().text =
				"가방으로 머리를 보호하세요!";
			warningUi.SetActive(true);
		}
		else
			warningUi.SetActive(false);

		//잡지 않고 있을 때 경고창 활성화
		if (doGrapDeskLeg == true &&
		    grapDeskCheck.GetComponent<DeskLeg>().isStillGrap == false)
		{
			grabDeskLegUI.SetActive(true);
		}
		else if (doGrapDeskLeg == true &&
		         grapDeskCheck.GetComponent<DeskLeg>().isStillGrap == true)
		{
			grabDeskLegUI.SetActive(false);
		}

		DetectHeadLowering(); // 고개 숙임 감지
	}

	// 캐릭터 순간이동 함수
	private void TeleportCharacters()
	{
		Vector3 newScale = Vector3.one * 0.5f; // 0.5로 스케일 설정

		// 플레이어 이동
		if (playerMovPos != null)
		{
			player.transform.position = playerMovPos.position;
			player.transform.rotation = playerMovPos.rotation;
			player.transform.localScale = newScale; // 스케일 수정
		}

		// 세티 이동
		if (setiMovPos != null)
		{
			seti.gameObject.transform.position = setiMovPos.position;
		}

		// NPC 이동
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcMovPos.Length > i && npcMovPos[i] != null)
			{
				NPC[i].transform.position = npcMovPos[i].position;
				NPC[i].transform.localScale = newScale; // 스케일 수정
			}
		}

		Debug.Log("플레이어 및 NPC 이동 및 스케일 조정 완료");
	}


	// 초기 위치로 복귀하는 함수
	public void ReturnToOriginalPosition()
	{
		Vector3 originalScale = Vector3.one; // 기본 스케일 (1,1,1)

		// 플레이어 위치 복귀
		if (playerSpawnPos != null)
		{
			player.transform.position = playerSpawnPos.position;
			player.transform.rotation = playerSpawnPos.rotation;
			player.transform.localScale = originalScale; // 스케일 원래대로
		}

		// NPC 위치 복귀
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcSpawnPos.Length > i && npcSpawnPos[i] != null)
			{
				NPC[i].transform.position = npcSpawnPos[i].position;
				NPC[i].transform.rotation = npcSpawnPos[i].rotation;
				NPC[i].transform.localScale = originalScale; // 스케일 원래대로
			}
		}

		Debug.Log("플레이어 및 NPC의 초기 위치 및 스케일로 복귀 완료");
	}


	// 부모 오브젝트의 모든 자식에게 Outlinable 컴포넌트 활성화
	private void ActiveOutlineToChildren(GameObject parent)
	{
		if (parent == null) return;

		foreach (Transform child in parent.GetComponentsInChildren<Transform>())
		{
			Highlighter highlighter = child.gameObject.GetComponent<Highlighter>();

			if (highlighter == null)
			{
				highlighter = child.gameObject.AddComponent<Highlighter>();
			}

			highlighter.SetColor(Color.yellow);
			highlighter.isBlinking = true; // Highlighter 활성화
		}
	}


	// 머리 숙임 감지 함수
	private void DetectHeadLowering()
	{
		if (xrCamera == null) return;

		float currentHeight = xrCamera.position.y; // 현재 머리 높이

		if (currentHeight < headHeightThreshold)
		{
			isHeadDown = true;
			// Debug.Log("머리를 숙였습니다! (Y축 기준)");
		}
		else
		{
			isHeadDown = false;
		}
	}

	// 모든 NPC의 상태를 변경하는 함수
	public void SetAllNpcState(NpcRig.State newState)
	{
		foreach (GameObject npc in NPC)
		{
			if (npc != null) // NPC가 null이 아닌지 확인
			{
				npc.GetComponent<NpcRig>().state = newState; // NPC의 상태 변경
			}
		}
	}

	// 선택지를 처리하는 함수
	void HandleChoice(bool isLeftChoice)
	{
		exampleDescUi.SetActive(false);

		if (isLeftChoice)
		{
			leftChoiceDialog.gameObject.SetActive(true);
			seti.SetAngry();
			Debug.Log("왼쪽 선택지가 선택됨");
		}
		else
		{
			rightChoiceDialog.gameObject.SetActive(true);
			seti.SetHappy();
			Debug.Log("오른쪽 선택지가 선택됨");
		}
	}

	void OkBtnClick()
	{
		if (isButtonClick == false)
		{
			isButtonClick = true;
			Debug.Log("버튼이 클릭되었습니다.");
		}
	}
}