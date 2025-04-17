using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBeginner : MonoBehaviour
{
	public enum PLACE
	{
		CLASSROOM,
		HALLWAY,
		STAIRS_ELEVATOR,
		OUTSIDE,
		HOUSE,
        DOWNSTAIR
    };

	[Header("시작 장소")] public PLACE place;

	[Header("화재 상황 여부")] public bool isFireBeginner;

	public bool isFireAdvanced;

	[Header("NPC 및 플레이어 이동 관련 설정")] public GameObject player; // 플레이어의 시작 위치
	public RobotController seti;
	public GameObject[] NPC; // 기타 NPC들의 시작 위치
	public FadeInOut fadeInOutImg;
	public Button okBtn;
	public GameObject warningUi;
	public GameObject exampleDescUi;
	public Canvas playerUi; //플레이어 UI Canvas

	[SerializeField]
	private GameObject emergencyExit; // 비상구 오브젝트 
    [SerializeField]
    private ExitLine advEmergencyExitLine;

	[SerializeField] private GameObject handkerchief;
	[SerializeField] private GameObject fireAlarm;
    [SerializeField] private GameObject exitUiObj;

	[Header("머리 위치 체크")] public bool isHeadDown = false;
	public float headHeightThreshold; // 머리 높이 기준 (이 값보다 낮으면 머리를 숙인 것으로 판단)

	[SerializeField] private Transform xrCamera; // HMD 카메라
	[SerializeField] private float initialHeight; // 초기 플레이어 높이

	[Header("이동 목표 위치")] public Transform playerMovPos;
	public Transform setiMovPos;
	public Transform[] npcMovPos;

	[Header("연기 파티클 그룹")] public GameObject smokeParticles;

	[Header("예제 UI 이미지")] 
	[SerializeField] private Image LeftImg;
	[SerializeField] private Image RightImg;
	[SerializeField] private Button LeftBtn;
	[SerializeField] private Button RightBtn;
	[SerializeField] private Sprite leftChangeImg;
	[SerializeField] private Sprite rightChangeImg;

	[Header("상황 진행 체크")] 
    public bool isFirstStepRdy;
	public bool isSecondStepRdy;
	public bool hasHandkerchief;
	public bool iscoverFace;
	public bool isInLivingRoom;
	public bool isInKitchen;
	public bool ruleCheck; //손수건을 획득한 후 경고창을 띄우기 위해 경고 지점을 설정하는 변수 (해당 변수가 true된 시점부터 경고가 출력)
    public bool canGetItems;    //아이템을 잡을 수 있는 상황
    public bool isButtonClick;  // 눌렸는지 확인하는 변수 

	[Header("대화 시스템")] 
    [SerializeField] private BeginnerDialogSystem firstDialog;
	[SerializeField] private BeginnerDialogSystem secondDialog;
	[SerializeField] private BeginnerDialogSystem thirdDialog;
	[SerializeField] private BeginnerDialogSystem forthDialog;
	[SerializeField] private BeginnerDialogSystem fifthDialog;
	[SerializeField] private BeginnerDialogSystem leftChoiceDialog;
	[SerializeField] private BeginnerDialogSystem rightChoiceDialog;

    private void Awake()
	{
		xrCamera = Camera.main.transform;
		initialHeight = xrCamera.position.y; // 초기 플레이어 높이 저장
	}

	// 게임 시작 시 실행
	IEnumerator Start()
	{
		if(isFireBeginner)
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

                    // 2. 화재 상황 안내 대화 시작 및 화재 경보 작동
                    secondDialog.gameObject.SetActive(true);
                    fireAlarm.SetActive(true);
                    Debug.Log("화재 경보 작동.");
                    SmokeObjsActive(); //모든 연기 파티클 활성화
                    seti.SetLookingFor();
                    handkerchief.SetActive(true);
                    yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);

                    // 3. OK 버튼 활성화 및 버튼 클릭 대기
                    okBtn.gameObject.SetActive(true);
                    Debug.Log("OK 버튼 활성화");
                    okBtn.GetComponent<Button>().onClick.AddListener(() => OkBtnClick());
                    yield return new WaitUntil(() => isButtonClick == true);
                    seti.SetBasic();
                    // 버튼 클릭 후 예제 UI 이미지 변경 및 첫 번째 단계 준비 완료
                    LeftImg.sprite = leftChangeImg;
                    RightImg.sprite = rightChangeImg;
                    isFirstStepRdy = true;
                    Debug.Log("예제 UI 첫 번째 단계 준비 완료");

                    // 4. 첫 단계 준비 완료 후 OK 버튼 비활성화 및 예제 UI 활성화
                    yield return new WaitUntil(() => isFirstStepRdy == true);
                    okBtn.gameObject.SetActive(false);
                    exampleDescUi.SetActive(true);
                    Debug.Log("OK 버튼 비활성화 및 예제 UI 활성화");
                    SetAllNpcState(NpcRig.State.Hold); // 모든 NPC 상태를 Hold로 설정

                    // 5. 손수건 사용 및 얼굴 가리기 완료 대기
                    yield return new WaitUntil(() =>
                        hasHandkerchief == true && iscoverFace == true);
                    Debug.Log("손수건 사용 및 얼굴 가리기 완료");
                    exampleDescUi.SetActive(false);

                    // 6. 페이드 인/아웃을 통한 NPC 및 플레이어 이동
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                    Debug.Log("플레이어 및 NPC 이동");
                    TeleportCharacters();
                    AdjustUiTransform();
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    isSecondStepRdy = true;
                    yield return new WaitUntil(() => isSecondStepRdy == true);

                    // 7. 추가 대화 진행 후 얼굴 가리기 완료 대기, 페이드 아웃 후 씬 전환
                    thirdDialog.gameObject.SetActive(true);
                    ruleCheck = true; //해당 시점부터 손수건 경고 출력
                    yield return new WaitUntil(() =>thirdDialog.isDialogsEnd == true && iscoverFace == true);
                    fadeInOutImg.gameObject.SetActive(true);
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    SceneManager.LoadScene("Fr_Kinder_2");
                    break;

                // 복도
                case PLACE.HALLWAY:
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    SetAllNpcState(NpcRig.State.Bow); // 모든 NPC 상태를 Bow로 설정
                    ruleCheck = true;
                    hasHandkerchief = true;
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeIn == false);
                    // 1. 첫 번째 대화 종료 대기
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                    // 2. 두 번째 대화 시작 및 비상구에 아웃라인 활성화
                    secondDialog.gameObject.SetActive(true);
                    ActiveOutlineToChildren(emergencyExit);
                    yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);

                    // 3. 세 번째 대화 시작 및 머리 숙이기, 얼굴 가리기 완료 대기 후 씬 전환
                    thirdDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
                    yield return new WaitUntil(() =>
                        isHeadDown == true && iscoverFace == true);

                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    SceneManager.LoadScene("Fr_Kinder_3");
                    break;

                // 계단/엘리베이터
                case PLACE.STAIRS_ELEVATOR:
                    ruleCheck = true;
                    hasHandkerchief = true;
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    SetAllNpcState(NpcRig.State.Bow); // 모든 NPC 상태를 Bow로 설정
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeIn == false);
                    // 1. 첫 번째 대화 종료 대기
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                    // 2. OK 버튼 활성화 후 클릭 대기
                    okBtn.gameObject.SetActive(true);
                    okBtn.GetComponent<Button>().onClick.AddListener(() => OkBtnClick());
                    yield return new WaitUntil(() => isButtonClick == true);
                    okBtn.gameObject.SetActive(false);
                    // 머리 숙이기와 얼굴 가리기 완료 대기 후 씬 전환
                    yield return new WaitUntil(() =>
                        isHeadDown == true && iscoverFace == true);
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    SceneManager.LoadScene("Fr_Kinder_4");
                    break;

                // 외부
                case PLACE.OUTSIDE:
                    seti.SetHappy();
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeIn == false);
                    // 외부에서는 첫 번째 대화만 실행
                    firstDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                    //대사 종료되면 FadeOut 진행 후 Title Scene 이동
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    isFirstStepRdy = true;
                    yield return new WaitUntil(() => isFirstStepRdy == true);
                    SceneManager.LoadScene(0); //Title로 복귀 
                    break;
            }
        }
		//화재 고급 시나리오 진행
		else if(isFireAdvanced)
		{
			switch(place)
			{
				case PLACE.HOUSE:
					isInLivingRoom = true;
					yield return new WaitUntil(() => isInLivingRoom == true);

                    // 1. Fade In, Out 진행 후 첫 번째 대화 시작 
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                    firstDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                    //2.상황창밖으로 검은 연기와 불길이 아랫 집에서 올라오는 장면을 비춰 줌(화재 비상벨이 시끄럽게 울리고 있음) 이후 두번째 대화 진행
                    //파티클 활성화
                    SmokeObjsActive();
                    seti.SetLookingFor();
                    //대화 이후 선택지 창 활성화
                    fireAlarm.gameObject.SetActive(true);
                    secondDialog.gameObject.SetActive(true);
					yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                    LeftImg.sprite = leftChangeImg;
                    RightImg.sprite = rightChangeImg;
                    isFirstStepRdy = true;
                    Debug.Log("예제 UI 첫 번째 단계 준비 완료");
                    yield return new WaitUntil(() => isFirstStepRdy == true);

                    // 4. 첫 단계 준비 완료 후 선택지 UI 출력 선택지에 따라 다른 대사 출력 후 마무리 대사진행
                    exampleDescUi.SetActive(true);
                    LeftBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(true));
                    RightBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(false));

                    yield return new WaitUntil(() => leftChoiceDialog.isDialogsEnd == true || rightChoiceDialog.isDialogsEnd == true);
                    seti.SetBasic();
                    thirdDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
                    //5. 세번째 대사 실행 후 주방으로 이동
                    isInLivingRoom = false;
					isInKitchen = true;
					yield return new WaitUntil(() => isInLivingRoom == false && isInKitchen == true);
                    //주방으로 이동 (fade in, out모두 실행 후 대사 출력)
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    //주방으로 이동
                    TeleportCharacters();
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
					forthDialog.gameObject.SetActive(true);
                    handkerchief.gameObject.SetActive(true);
                    ruleCheck = true;
                    yield return new WaitUntil(() => forthDialog.isDialogsEnd == true && iscoverFace == true);

                    //마지막 대사 출력 후 이동
                    fifthDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true && iscoverFace == true);
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    //Scene 이동
                    SceneManager.LoadScene("Fr_Home_2");
						break;

				case PLACE.STAIRS_ELEVATOR:
                    ruleCheck = true;
                    SetAllNpcState(NpcRig.State.Bow);
                    fireAlarm.gameObject.SetActive(true);
                    // 1. Fade In, Out 진행 후 첫 번째 대화 시작 
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                    firstDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                    LeftImg.sprite = leftChangeImg;
                    RightImg.sprite = rightChangeImg;
                    isFirstStepRdy = true;
                    Debug.Log("예제 UI 첫 번째 단계 준비 완료");
                    yield return new WaitUntil(() => isFirstStepRdy == true);

                    //2. 대화 종료 후 선택지 UI 출력(선택지에 따라 다른 대사 출력)
                    exampleDescUi.SetActive(true);
                    LeftBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(true));
                    RightBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(false));
                    yield return new WaitUntil(() => leftChoiceDialog.isDialogsEnd == true || rightChoiceDialog.isDialogsEnd == true);
                    seti.SetBasic();
                    //3. 피난 유도선 Ray로 선택 시 Outline 강조, 대사종료 후 놀이터 Scene으로 이동
                    secondDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                    // ExitLineInteraction 호출
                    advEmergencyExitLine.ExitLineInteraction();

                    thirdDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => thirdDialog.isDialogsEnd);

                    // 선택 완료까지 대기
                    exitUiObj.SetActive(true);
                    yield return new WaitUntil(() => advEmergencyExitLine.isSelected == true);
                    Debug.Log("비상구 유도선 선택 완료!");
                    //비활성화
                    exitUiObj.SetActive(false);

                    isSecondStepRdy = true;
					yield return new WaitUntil(() => isSecondStepRdy == true);

                    //머리 숙이고 코를 막아야 이동
                    yield return new WaitUntil(() => isHeadDown == true && iscoverFace == true && isHeadDown == true);
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => FadeInOut.Instance.isFadeOut == false);
                    SceneManager.LoadScene("Fr_Home_3");
                    break;

                case PLACE.DOWNSTAIR:
                    ruleCheck = true;
                    SetAllNpcState(NpcRig.State.Bow);
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);

                    firstDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                    //두번째 대사 출력
                    secondDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);

                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                    SceneManager.LoadScene("Fr_Home_4");
                    break;

				case PLACE.OUTSIDE:
                    // 1. Fade In, Out 진행 후 첫 번째 대화 시작,대화 종료 후 TitleScene으로 이동
                    //사이렌 소리를 들려줌
                    StartCoroutine(FadeInOut.Instance.FadeIn());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                    seti.SetHappy();
                    firstDialog.gameObject.SetActive(true);
                    yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                    seti.SetBasic();
                    StartCoroutine(FadeInOut.Instance.FadeOut());
                    yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
					SceneManager.LoadScene(0);
                    break;

			}
        }
		
	}

	private void Update()
	{
		//손수건 획득 후 손수건으로 입을 잘 가리고 있는지 확인
		if (ruleCheck == true && hasHandkerchief == true && iscoverFace == false && isHeadDown == true)
        {
            warningUi.GetComponentInChildren<TextMeshProUGUI>().text = "손수건으로 코를 막으세요!";
            warningUi.SetActive(true);
        }
        else if(ruleCheck == true && hasHandkerchief == true && iscoverFace == true && isHeadDown == false)
        {
            warningUi.GetComponentInChildren<TextMeshProUGUI>().text = "몸을 숙이세요!";
            warningUi.SetActive(true);
        }
        else if(ruleCheck == true && hasHandkerchief == true && iscoverFace == false && isHeadDown == false)
        {
            warningUi.GetComponentInChildren<TextMeshProUGUI>().text = "손수건으로 코를 막고 몸을 숙이세요!";
            warningUi.SetActive(true);
        }
        else
        {
            Debug.Log("모든 행동을 만족했습니다.");
            warningUi.SetActive(false);
        }

		DetectHeadLowering(); // 머리 숙임 감지
	}
    //이동 후 Ui위치 변경

    public void AdjustUiTransform()
    {
        if (playerUi == null || player == null) return;

        RectTransform rectTransform = playerUi.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // 월드 기준 위치 설정
        Vector3 worldPosition = player.transform.position +
                                player.transform.TransformDirection(
                                    new Vector3(0f, 1f, 0.6f));
        rectTransform.position = worldPosition;

        // 로컬 회전 설정 (주어진 값 그대로 적용)
        rectTransform.localRotation =
            Quaternion.Euler(0f, 0f, 0f); // 월드 기준이 아닌, 그대로 적용

        Debug.Log("playerUi 위치 및 회전 변경 완료 (Local Space)");
    }

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
            seti.transform.rotation = setiMovPos.rotation;
		}

		// NPC 이동
		for (int i = 0; i < NPC.Length; i++)
		{
			if (npcMovPos.Length > i && npcMovPos[i] != null)
			{
				NPC[i].transform.position = npcMovPos[i].position;
                NPC[i].transform.rotation = npcMovPos[i].rotation;
			}
		}

		Debug.Log("플레이어 및 NPC 이동 완료");
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
			// Debug.Log("머리를 숙였습니다! (Y 값 기준)");
		}
		else
		{
			isHeadDown = false;
		}
	}

	// 화재 발생 시 모든 연기 파티클 활성화
	public void SmokeObjsActive()
	{
		if (smokeParticles == null) return;

		// 1. 모든 자식 오브젝트 활성화
		foreach (Transform child in smokeParticles
			         .GetComponentsInChildren<Transform>(true)) // 비활성화된 오브젝트도 포함
		{
			child.gameObject.SetActive(true);
		}

		// 2. 자식 오브젝트들 중 ParticleSystem 찾아서 실행
		foreach (ParticleSystem particle in smokeParticles
			         .GetComponentsInChildren<ParticleSystem>(true))
		{
			particle.Play(); // 파티클 실행
		}

		Debug.Log("모든 연기 파티클이 활성화되었습니다.");
	}

	// 모든 NPC들의 상태를 동일하게 변경하는 함수
	public void SetAllNpcState(NpcRig.State newState)
	{
		foreach (GameObject npc in NPC)
		{
			if (npc != null) // NPC가 null이 아닌지 확인
			{
				npc.GetComponent<NpcRig>().state = newState; // NPC의 상태를 변경
			}
		}
	}
    void HandleChoice(bool isLeftChoice)
    {
        exampleDescUi.SetActive(false);

        if (isLeftChoice)
        {
            leftChoiceDialog.gameObject.SetActive(true);
            seti.SetAngry();
            Debug.Log("왼쪽 선택지 대사 출력");
        }
        else
        {
            rightChoiceDialog.gameObject.SetActive(true);
            seti.SetHappy();
            Debug.Log("오른쪽 선택지 대사 출력");
        }
    }
    void OkBtnClick()
    {
        if(isButtonClick == false)
        {
            isButtonClick = true;
            Debug.Log("버튼이 클릭되었습니다.");
        }

    }
}