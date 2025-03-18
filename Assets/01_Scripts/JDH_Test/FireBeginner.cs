using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBeginner : MonoBehaviour
{
    public enum PLACE { CLASSROOM, HALLWAY, STAIRS_ELEVATOR, OUTSIDE };

    [Header("장소 상태")]
    public PLACE place;

    [Header("화재인지 지진인지 확인하는 변수")]
    public bool isFireBeginner;
    public bool isEarthquake;

    [Header("NPC 및 상호작용 오브젝트")]
    public GameObject player;   //싱글 플레이어 위치
    public GameObject seti;
    public GameObject[] NPC;    //멀티 플레이어 위치
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject exampleDescUi;
    public GameObject leftHand; //왼손 오브젝트
    [SerializeField] private GameObject emergencyExit;    //자식 오브젝트에 addcomponent를 통해 Outlinable을 부착
    [SerializeField] private GameObject handkerchief;
    [SerializeField] private GameObject fireAlarm;

    [Header("머리 숙임 감지 변수")]
    public bool isHeadDown = false;
    public float headHeightThreshold; // 기준 높이 (이 값보다 낮아지면 숙인 것으로 판단)

    [SerializeField] private Transform xrCamera;  // HMD 카메라 트래킹
    [SerializeField]private float initialHeight; // 초기 플레이어 높이 저장

    [Header("상호작용 혹은 위치 이동 지점")]
    public Transform playerMovPos;
    public Transform setiMovPos;
    public Transform[] npcMovPos;

    [Header("사용하는 파티클")]
    public ParticleSystem smokeParticle;

    [Header("ExampleUI 변경 이미지 관련 변수")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("진행상황 체크 변수")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;
    public bool hasHandkerchief;
    public bool iscoverFace;

    [Header("시나리오 대사 목록")]
    [SerializeField] private BeginnerDialogSystem firstDialog;
    [SerializeField] private BeginnerDialogSystem secondDialog;
    [SerializeField] private BeginnerDialogSystem thirdDialog;

    private void Awake()
    {
        xrCamera = Camera.main.transform;
        initialHeight = xrCamera.position.y; // 게임 시작 시 초기 높이 저장
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        switch (place)
        {
            //교실
            case PLACE.CLASSROOM:
                fadeInOutImg.gameObject.SetActive(false);
                //fadeInOutImg.StartCoroutine(fadeInOutImg.FadeIn());
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                //화재 경보음과 함께 두번째 시나리오 대사 출력
                secondDialog.gameObject.SetActive(true);
                //화재 경보음 출력
                fireAlarm.SetActive(true);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                //대사 종료 후 버튼 활성화, 버튼 누르기 전까지 대기
                okBtn.gameObject.SetActive(true);

                yield return new WaitUntil(() => okBtn.isHovered == true);
                //다음 진행은 isFirstStepRdy 가 true일 때 까지 대기한다. (이미지 변경)
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                isFirstStepRdy = true;

                yield return new WaitUntil(() => isFirstStepRdy == true);
                okBtn.gameObject.SetActive(false);
                exampleDescUi.SetActive(true);
                //책상에 손수건 생성
                handkerchief.gameObject.SetActive(true);
                //손으로 손수건 잡으면 왼손에 고정 (고정할 때 까지 대기)
                yield return new WaitUntil(() => hasHandkerchief == true);
                Debug.Log("손수건 고정 완료");
                //유저 입 주변에 손수건 접촉 시 입과 코를 가린 것으로 판정

                yield return new WaitUntil(() => iscoverFace == true);
                //FadeIn, Out으로 이동하는 모습을 안보여준다.
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                // 플레이어와 NPC의 위치를 문 앞으로 이동, 세티 또한 위치 변경
                Debug.Log("플레이어 NPC 위치 이동");
                TeleportCharacters();
                StartCoroutine(fadeInOutImg.FadeIn());

                //플레이어와 NPC가 이동하고 입과 코를 가린 것으로 판정되면
                isSecondStepRdy = true;
                //isSecondStepRdy가 true가 되면 세번째 시나리오 대사 출력
                yield return new WaitUntil(() => isSecondStepRdy == true);
                thirdDialog.gameObject.SetActive(true);
                fadeInOutImg.gameObject.SetActive(true);
                //Fade Out 진행 된 후 Scene 이동
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동
                SceneManager.LoadScene("JDH2");
                break;

            //복도
            case PLACE.HALLWAY:
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                secondDialog.gameObject.SetActive(true);
                //피난 유도선 테두리 강조
                ActiveOutlineToChildren(emergencyExit);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                thirdDialog.gameObject.SetActive(true);
                //플레이어가 손수건을 통해 입과 코를 잘 막고있는지 확인(경고 UI 출력: 손수건으로 입과 코를 가려줘!)


                //Fade Out 진행 된 후 Scene 이동
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동
                SceneManager.LoadScene("JDH3");
                break;

            //계단, 엘레베이터
            case PLACE.STAIRS_ELEVATOR:
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동(경고 UI 출력: 손수건으로 입과 코를 가려줘!)
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                //버튼 클릭 대기

                //Fade Out 진행 된 후 Scene 이동
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH4");
                break;

            //유치원 밖 놀이터
            case PLACE.OUTSIDE:
                //마무리 대사만 출력 후 종료
                firstDialog.gameObject.SetActive(true);
                break;

        }
    }
    private void Update()
    {
        DetectHeadLowering(); // 머리 높이 감지 함수 호출
    }

    private void TeleportCharacters()
    {
        // 플레이어 즉시 이동
        if (playerMovPos != null)
        {
            player.transform.position = playerMovPos.position;
        }

        // 세티 즉시 이동
        if (setiMovPos != null)
        {
            seti.transform.position = setiMovPos.position;
        }

        // NPC 즉시 이동
        for (int i = 0; i < NPC.Length; i++)
        {
            if (npcMovPos.Length > i && npcMovPos[i] != null)
            {
                NPC[i].transform.position = npcMovPos[i].position;
            }
        }

        Debug.Log("플레이어 및 NPC 텔레포트 완료");
    }

    //자식 오브젝트에 Outlinable을 활성화
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
    private void DetectHeadLowering()
    {
        if (xrCamera == null) return;

        float currentHeight = xrCamera.position.y; // 현재 머리 높이

        // 머리 높이가 기준보다 낮아지면 숙인 것으로 판단
        if (currentHeight < headHeightThreshold)
        {
            isHeadDown = true;
            Debug.Log("머리를 숙였습니다! (Y값 감지)");
        }
        else
        {
            isHeadDown = false;
        }
    }

}
