using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireBeginner : MonoBehaviour
{
    public enum PLACE { CLASSROOM, HALLWAY, STAIRS_ELEVATOR, OUTSIDE };

    [Header("장소 상태")]
    public PLACE place;
    [Header("NPC 및 상호작용 오브젝트")]
    public GameObject[] NPC;
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject exampleDescUi;
    public GameObject Handkerchief;

    [Header("상호작용 혹은 위치 이동 지점")]
    public Transform HandkerchiefSpawnPos;

    [Header("ExampleUI 변경 이미지 관련 변수")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("진행상황 체크 변수")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;

    [Header("시나리오 대사 목록")]
    [SerializeField] private BeginnerDialogSystem firstDialog;
    [SerializeField] private BeginnerDialogSystem secondDialog;
    [SerializeField] private BeginnerDialogSystem thirdDialog;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        switch (place)
        {
            case PLACE.CLASSROOM:
                //fadeInOutImg.StartCoroutine(fadeInOutImg.FadeIn());
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                //화재 경보음과 함께 두번째 시나리오 대사 출력
                secondDialog.gameObject.SetActive(true);
                //화재 경보음 출력

                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                //대사 종료 후 버튼 활성화, 버튼 누르기 전까지 대기
                okBtn.gameObject.SetActive(true);
                //다음 진행은 isFirstStepRdy 가 true일 때 까지 대기한다. (이미지 변경)
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                yield return new WaitUntil(() => isFirstStepRdy == true);
                exampleDescUi.SetActive(true);

                //책상에 손수건 생성
                GameObject HandkerchiefObj = Instantiate(Handkerchief, HandkerchiefSpawnPos.position, Quaternion.identity);
                //손으로 손수건 잡으면 왼손에 고정
                Debug.Log(":)");
                //유저 입 주변에 손수건 접촉 시 입과 코를 가린 것으로 판정

                // 플레이어와 NPC의 위치를 문 앞으로 이동, 세티 또한 위치 변경

                //왼손이 지정한 범위에서 떨어질 경우 손수건이 떨어진 판정(경고 UI 출력: 손수건으로 입과 코를 가려줘!)   

                //플레이어와 NPC가 이동하고 입과 코를 가린 것으로 판정되면
                isSecondStepRdy = true;
                //isSecondStepRdy가 true가 되면 세번째 시나리오 대사 출력
                yield return new WaitUntil(() => isSecondStepRdy == true);
                thirdDialog.gameObject.SetActive(true);

                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동
                SceneManager.LoadScene("JDH2");
                break;

            case PLACE.HALLWAY:
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                secondDialog.gameObject.SetActive(true);
                //피난 유도선 테두리 강조
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                thirdDialog.gameObject.SetActive(true);
                //플레이어가 손수건을 통해 입과 코를 잘 막고있는지 확인
                
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동
                SceneManager.LoadScene("JDH3");
                break;

            case PLACE.STAIRS_ELEVATOR:
                //모든 진행이 완료되었기에 버튼 클릭 시 다음 씬으로 이동
                SceneManager.LoadScene("JDH4");
                break;

            case PLACE.OUTSIDE:
                //마무리 대사만 출력 후 종료
                break;

        }
    }
}
