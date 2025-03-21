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

    [Header("지진 상황 여부")] public bool isEarthquakeBeginner;

    [Header("NPC 및 플레이어 이동 관련 설정")] public GameObject player; // 플레이어의 시작 위치
    public GameObject seti;
    public GameObject[] NPC; // 기타 NPC들의 시작 위치
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject warningUi;
    public GameObject exampleDescUi;
    public GameObject leftHand; // 왼손 관련 오브젝트
    public Canvas playerUi; //플레이어 UI Canvas

    [SerializeField]
    private GameObject emergencyExit; // 비상구 오브젝트 (Outlinable 컴포넌트를 추가할 대상)
    [SerializeField] private GameObject fireAlarm;

    [Header("머리 위치 체크")] 
    public bool isHeadDown = false;
    public float headHeightThreshold; // 머리 높이 기준 (이 값보다 낮으면 머리를 숙인 것으로 판단)

    [SerializeField] private Transform xrCamera; // HMD 카메라
    [SerializeField] private float initialHeight; // 초기 플레이어 높이

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

                // 3. 두 번째 대화 시작
                secondDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                // 4. 책상 밑으로 들어가기 선택지 UI 발생 UI를 누르면 책상 밑으로 시점 변환 이후 대사 진행

                thirdDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);

                // 5. 유저 앞에있는 책상 다리의 아웃라인이 활성화 이후 대사 출력
                forthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => forthDialog.isDialogsEnd == true);

                // 6. 책상 옆에 떨어져 있는 가방의 테두리를 강조하며 아이들에게 위치를 확인시킨다.
                fifthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => fifthDialog.isDialogsEnd == true);
                // 7. 흔들림이 멈춤 이후 대사 출력
                sixthDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => sixthDialog.isDialogsEnd == true);

                // 8. 책상 옆에 떨어져 있는 가방의 테두리를 강조시키며 아이들에게 선택하게 한다. 선택 이후 대사 출력
                seventhDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => seventhDialog.isDialogsEnd == true);

                //9. Scene이동
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth2");
                break;

            case PLACE.HALLWAY:
                //대사 출력 후 계단Scene으로 이동
                StartCoroutine(FadeInOut.Instance.FadeIn());
                yield return new WaitUntil(() => fadeInOutImg.isFadeIn == false);
                firstDialog.gameObject.SetActive(true);
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                StartCoroutine(FadeInOut.Instance.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH_Earth3");
                break;
            case PLACE.STAIRS_ELEVATOR:

                break;
            case PLACE.OUTSIDE:

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