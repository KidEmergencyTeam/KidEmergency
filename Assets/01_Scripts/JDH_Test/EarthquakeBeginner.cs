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
        CLASSROOM, // 교실
        HALLWAY, // 복도
        STAIRS_ELEVATOR, // 계단/엘리베이터
        OUTSIDE, // 건물 밖
        HOUSE // 집
    };

    [Header("현재 위치")]
    public PLACE place;

    [Header("지진 시작 여부")]
    public bool isEarthquakeStart;

    [Header("NPC 및 플레이어 이동 관련 오브젝트")]
    public GameObject player; // 플레이어 캐릭터 오브젝트
    public GameObject seti;
    public GameObject[] NPC; // NPC 캐릭터 오브젝트 배열
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject warningUi;
    public GameObject exampleDescUi;
    public GameObject leftHand; // 메뉴 조작 핸드 오브젝트
    public Canvas playerUi; // 플레이어 UI Canvas

    [Header("가방, 출구, 경보 장치 오브젝트")]
    [SerializeField] private GameObject backpack;
    [SerializeField] private GameObject emergencyExit; // 비상 출구 (Outlinable 컴포넌트 추가 필요)
    [SerializeField] private GameObject fireAlarm;
    public EarthquakeSystem earthquake;

    [Header("고개 숙임 여부 체크")]
    public bool isHeadDown = false;
    public float headHeightThreshold; // 고개 숙임을 판단하는 높이 기준
    [SerializeField] private Transform xrCamera; // HMD 카메라
    [SerializeField] private float initialHeight; // 초기 플레이어 높이

    [Header("플레이어 및 NPC 스폰 위치")]
    [SerializeField] private Transform playerSpawnPos;
    [SerializeField] private Transform[] npcSpawnPos;

    [Header("이동 목표 위치")]
    public Transform playerMovPos;
    public Transform setiMovPos;
    public Transform[] npcMovPos;

    [Header("선택 UI 이미지 및 버튼")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private TestButton2 LeftBtn;
    [SerializeField] private TestButton2 RightBtn;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("진행 상태 체크 변수")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;
    public bool hasHandkerchief;
    public bool ruleCheck;

    [Header("대화 시스템")]
    [SerializeField] private BeginnerDialogSystem firstDialog;
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

                // 2. 화면이 흔들리며 지진 발생. 화재 경보도 함께 울림.
                isEarthquakeStart = true;
                earthquake.StartEarthquake();
                fireAlarm.gameObject.SetActive(true);

                // 3. 두 번째 대화 시작
                secondDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);

                // 4. 가방을 챙기라는 메시지를 띄우는 UI 활성화
                okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "가방 챙기기";
                okBtn.gameObject.SetActive(true);
                yield return new WaitUntil(() => okBtn.isClick == true);
                okBtn.gameObject.SetActive(false);
                okBtn.isClick = false;

                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);

                // 플레이어와 NPC 이동 및 행동 변경
                TeleportCharacters();
                SetAllNpcState(NpcRig.State.DownDesk);
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == true);

                thirdDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);

                // 5. 책상 아래로 대피하는 장면 연출
                forthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => forthDialog.isDialogsEnd == true);
                SetAllNpcState(NpcRig.State.DownDesk);

                // 6. 가방을 챙기도록 유도하는 이벤트 발생
                backpack.GetComponent<Outlinable>().enabled = true;
                fifthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true);

                // 7. 지진 종료 (현재 주석 처리됨)
                // earthquake.StopEarthquake();
                // yield return new WaitUntil(() => earthquake._endEarthquake == true);

                sixthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);

                // 가방 챙기기 버튼 다시 활성화
                okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "가방 챙기기";
                okBtn.gameObject.SetActive(true);
                yield return new WaitUntil(() => okBtn.isClick == true);
                okBtn.gameObject.SetActive(false);

                // 플레이어와 NPC를 원래 위치로 이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                ReturnToOriginalPosition();
                SetAllNpcState(NpcRig.State.None);
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);

                // 8. 대피 후 정리 단계
                seventhDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => seventhDialog.isDialogsEnd == true);

                // 9. 다음 씬으로 이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth2");
                break;

            // 복도
            case PLACE.HALLWAY:
                SetAllNpcState(NpcRig.State.HoldBag);

                // 첫 번째 대화 후 씬 이동
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                // 씬 전환
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth3");
                break;

            // 계단과 엘리베이터
            case PLACE.STAIRS_ELEVATOR:
                SetAllNpcState(NpcRig.State.HoldBag);

                // 1. 첫 번째 대화 시작
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                // 2. 선택지 이미지 변경 및 버튼 활성화
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                isFirstStepRdy = true;
                yield return new WaitUntil(() => isFirstStepRdy == true);

                // 3. 선택지 UI 표시
                exampleDescUi.gameObject.SetActive(true);
                LeftBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(true));
                RightBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(false));

                yield return new WaitUntil(() =>
                    leftChoiceDialog.isDialogsEnd == true ||
                    rightChoiceDialog.isDialogsEnd == true);

                // 4. 다음 대화 진행
                thirdDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);

                // 씬 전환
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth4");
                break;

            // 건물 밖
            case PLACE.OUTSIDE:
                // 1. 첫 번째 대화 시작
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                // 씬 전환 (타이틀 화면으로 이동)
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene(0);
                break;
        }
    }


    private void Update()
    {
        DetectHeadLowering(); // 고개 숙임 감지
    }

    // 이동 후 UI 위치 조정
    public void AdjustUiTransform()
	{
		if (playerUi == null || player == null) return;

		RectTransform rectTransform = playerUi.GetComponent<RectTransform>();
		if (rectTransform == null) return;

		// UI의 월드 위치 설정
		Vector3 worldPosition = player.transform.position +
								player.transform.TransformDirection(
									new Vector3(0.75f, 1.5f, 0.5f));
		rectTransform.position = worldPosition;

		// UI 회전 설정 (임의의 값 적용)
		rectTransform.localRotation =
			Quaternion.Euler(-20, 50, 0); // 플레이어를 따라가는 것이 아니라 고정된 회전

		Debug.Log("playerUi 위치 및 회전 조정 완료 (Local Space)");
	}

	// 캐릭터 순간이동 함수
	private void TeleportCharacters()
	{
		// 플레이어 이동
		if (playerMovPos != null)
		{
			player.transform.position = playerMovPos.position;
			player.transform.rotation = playerMovPos.rotation;
		}

		// 세티 이동
		if (setiMovPos != null)
		{
			seti.transform.position = setiMovPos.position;
		}

		// NPC 이동
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcMovPos.Length > i && npcMovPos[i] != null)
			{
				NPC[i].transform.position = npcMovPos[i].position;
			}
		}

		Debug.Log("플레이어 및 NPC 이동 완료");
	}

	// 초기 위치로 복귀하는 함수
	public void ReturnToOriginalPosition()
	{
		// 플레이어 위치 복귀
		if (playerSpawnPos != null)
		{
			player.transform.position = playerSpawnPos.position;
			player.transform.rotation = playerSpawnPos.rotation;
		}

		// NPC 위치 복귀
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcSpawnPos.Length > i && npcSpawnPos[i] != null)
			{
				NPC[i].transform.position = npcSpawnPos[i].position;
				NPC[i].transform.rotation = npcSpawnPos[i].rotation;
			}
		}

		Debug.Log("플레이어 및 NPC의 초기 위치로 복귀 완료");
	}

	// 부모 오브젝트의 모든 자식에게 Outlinable 컴포넌트 활성화
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

			outline.enabled = true; // Outlinable 활성화
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
			Debug.Log("왼쪽 선택지가 선택됨");
		}
		else
		{
			rightChoiceDialog.gameObject.SetActive(true);
			Debug.Log("오른쪽 선택지가 선택됨");
		}
	}
}
