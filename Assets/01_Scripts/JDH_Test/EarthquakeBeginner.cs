using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

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

    [Header("시작 장소")] public PLACE place;

    [Header("지진 상황 여부")] public bool isEarthquakeStart;

    [Header("NPC 및 플레이어 이동 관련 설정")] public GameObject player; // 플레이어의 시작 위치
    public GameObject seti;
    public GameObject[] NPC; // 기타 NPC들의 시작 위치
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject warningUi;
    public GameObject exampleDescUi;
    public GameObject leftHand; // 왼손 관련 오브젝트
    public Canvas playerUi; //플레이어 UI Canvas
    [Header("가방, 탈출구, 비상벨 오브젝트")]
    [SerializeField] private GameObject backpack;
    [SerializeField]
    private GameObject emergencyExit; // 비상구 오브젝트 (Outlinable 컴포넌트를 추가할 대상)
    [SerializeField] private GameObject fireAlarm;
    public EarthquakeSystem earthquake;

    [Header("머리 위치 체크")] 
    public bool isHeadDown = false;
    public float headHeightThreshold; // 머리 높이 기준 (이 값보다 낮으면 머리를 숙인 것으로 판단)

    [SerializeField] private Transform xrCamera; // HMD 카메라
    [SerializeField] private float initialHeight; // 초기 플레이어 높이

    [Header("원래 생성 위치")]
    [SerializeField] private Transform playerSpawnPos;
    [SerializeField] private Transform[] npcSpawnPos;
    [Header("이동 목표 위치")]
    public Transform playerMovPos;
    public Transform setiMovPos;
    public Transform[] npcMovPos;

    [Header("예제 UI 이미지")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private TestButton2 LeftBtn;
    [SerializeField] private TestButton2 RightBtn;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("상황 진행 체크")] public bool isFirstStepRdy;
    public bool isSecondStepRdy;
    public bool hasHandkerchief;
    public bool ruleCheck;

    [Header("대화 시스템")][SerializeField] private BeginnerDialogSystem firstDialog;
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
        initialHeight = xrCamera.position.y; // 초기 플레이어 높이 저장
        earthquake = FindObjectOfType<EarthquakeSystem>();
    }

    // 게임 시작 시 실행
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
                //2. 화면이 조금씩 흔들리며 경보음이 울리고 서랍장 같은 물체가 넘어지고 지진 소리가 시작됨
                isEarthquakeStart = true;
                //지진 시작
                earthquake.StartEarthquake();
                fireAlarm.gameObject.SetActive(true);
                // 3. 두 번째 대화 시작
                secondDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                // 4. 책상 밑으로 들어가기 선택지 UI 발생 UI를 누르면 책상 밑으로 시점 변환 이후 대사 진행
                okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "책상 밑으로";
                okBtn.gameObject.SetActive(true);
                yield return new WaitUntil(() => okBtn.isClick == true);
                okBtn.gameObject.SetActive(false);
                okBtn.isClick = false;
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                //플레이어와 NPC의 좌표 이동 및 행동 변화
                TeleportCharacters();
                SetAllNpcState(NpcRig.State.DownDesk);
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == true);
                thirdDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);

                // 5. 유저 앞에있는 책상 다리의 아웃라인이 활성화 이후 대사 출력
                //아웃라인 활성화
                forthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => forthDialog.isDialogsEnd == true);
                SetAllNpcState(NpcRig.State.DownDesk);  //NPC 행동 변경
                //책상 다리 잡기

                // 6. 책상 옆에 떨어져 있는 가방의 테두리를 강조하며 아이들에게 위치를 확인시킨다.
                //가방 오브젝트 아웃라인 활성화
                backpack.GetComponent<Outlinable>().enabled = true;
                fifthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true);
                // 7. 흔들림이 멈춤 이후 대사 출력
                //흔들림 멈춤
                earthquake.StopEarthquake(); // 지진 종료
                yield return new WaitUntil(() => earthquake._endEarthquake == true);

                sixthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);
                //책상 밖으로 버튼 활성화 및 텍스트 변경
                okBtn.GetComponentInChildren<TextMeshProUGUI>().text = "책상 밖으로";
                okBtn.gameObject.SetActive(true);
                yield return new WaitUntil(() => okBtn.isClick == true);
                okBtn.gameObject.SetActive(false);
                //원래 위치로 이동 및 NPC 모습 변경
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                ReturnToOriginalPosition(); //원래 위치로 이동
                SetAllNpcState(NpcRig.State.None);
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                // 8. 책상 옆에 떨어져 있는 가방의 테두리를 강조시키며 아이들에게 선택하게 한다. 선택 이후 대사 출력
                seventhDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => seventhDialog.isDialogsEnd == true);
                //가방을 잡으면 손에 부착된다. 머리 위치까지 올리면 마무리 대사 출력 후 fadeout으로 Scene 이동, 가방을 잡을 시 NPC 상태 변경

                //9. Scene이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth2");
                break;

            case PLACE.HALLWAY:
                SetAllNpcState((NpcRig.State.HoldBag));
                //대사 출력 후 계단Scene으로 이동
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                //피난 유도선 선택 컨트롤러로 select하면 outliner 활성화
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth3");
                break;

            case PLACE.STAIRS_ELEVATOR:
                SetAllNpcState((NpcRig.State.HoldBag));
                // 1. 첫 번째 대화 시작
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                //미리 선택지 이미지 변경
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                isFirstStepRdy = true;
                yield return new WaitUntil(() => isFirstStepRdy == true);

                // 2.계단과 엘리베이터 선택지가 유저에게 보여짐, 버튼 선택 후 대사 진행
                exampleDescUi.gameObject.SetActive(true);
                //
                LeftBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(true));
                RightBtn.GetComponent<Button>().onClick.AddListener(() => HandleChoice(false));
                yield return new WaitUntil(() => leftChoiceDialog.isDialogsEnd == true || rightChoiceDialog.isDialogsEnd == true);

                //3.세 번째 대화 진행 이후
                thirdDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
                //Scene이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth4");
                break;
            case PLACE.OUTSIDE:
                // 1. 첫 번째 대화 시작
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                //Scene이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene(0);  //타이틀로 이동
                break;
        }

    }
    private void Update()
    {
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
                                    new Vector3(0.75f, 1.5f, 0.5f));
        rectTransform.position = worldPosition;

        // 로컬 회전 설정 (주어진 값 그대로 적용)
        rectTransform.localRotation =
            Quaternion.Euler(-20, 50, 0); // 월드 기준이 아닌, 그대로 적용

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
    // 원래 위치로 되돌리는 함수
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

        Debug.Log("플레이어 및 NPC가 원래 위치로 복귀했습니다.");
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
            // Debug.Log("머리를 숙였습니다! (Y 값 기준)");
        }
        else
        {
            isHeadDown = false;
        }
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
            Debug.Log("왼쪽 선택지 대사 출력");
        }
        else
        {
            rightChoiceDialog.gameObject.SetActive(true);
            Debug.Log("오른쪽 선택지 대사 출력");
        }
    }
}