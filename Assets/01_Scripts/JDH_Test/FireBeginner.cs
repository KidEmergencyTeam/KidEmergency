using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireBeginner : MonoBehaviour
{
    public enum PLACE { CLASSROOM, HALLWAY, STAIRS_ELEVATOR, OUTSIDE };

    [Header("��� ����")]
    public PLACE place;
    [Header("NPC �� ��ȣ�ۿ� ������Ʈ")]
    public GameObject[] NPC;
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject exampleDescUi;
    public GameObject Handkerchief;

    [Header("��ȣ�ۿ� Ȥ�� ��ġ �̵� ����")]
    public Transform HandkerchiefSpawnPos;

    [Header("ExampleUI ���� �̹��� ���� ����")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("�����Ȳ üũ ����")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;

    [Header("�ó����� ��� ���")]
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
                //ȭ�� �溸���� �Բ� �ι�° �ó����� ��� ���
                secondDialog.gameObject.SetActive(true);
                //ȭ�� �溸�� ���

                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                //��� ���� �� ��ư Ȱ��ȭ, ��ư ������ ������ ���
                okBtn.gameObject.SetActive(true);
                //���� ������ isFirstStepRdy �� true�� �� ���� ����Ѵ�. (�̹��� ����)
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                yield return new WaitUntil(() => isFirstStepRdy == true);
                exampleDescUi.SetActive(true);

                //å�� �ռ��� ����
                GameObject HandkerchiefObj = Instantiate(Handkerchief, HandkerchiefSpawnPos.position, Quaternion.identity);
                //������ �ռ��� ������ �޼տ� ����
                Debug.Log(":)");
                //���� �� �ֺ��� �ռ��� ���� �� �԰� �ڸ� ���� ������ ����

                // �÷��̾�� NPC�� ��ġ�� �� ������ �̵�, ��Ƽ ���� ��ġ ����

                //�޼��� ������ �������� ������ ��� �ռ����� ������ ����(��� UI ���: �ռ������� �԰� �ڸ� ������!)   

                //�÷��̾�� NPC�� �̵��ϰ� �԰� �ڸ� ���� ������ �����Ǹ�
                isSecondStepRdy = true;
                //isSecondStepRdy�� true�� �Ǹ� ����° �ó����� ��� ���
                yield return new WaitUntil(() => isSecondStepRdy == true);
                thirdDialog.gameObject.SetActive(true);

                yield return new WaitUntil(() => thirdDialog.isDialogsEnd == true);
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�
                SceneManager.LoadScene("JDH2");
                break;

            case PLACE.HALLWAY:
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                secondDialog.gameObject.SetActive(true);
                //�ǳ� ������ �׵θ� ����
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                thirdDialog.gameObject.SetActive(true);
                //�÷��̾ �ռ����� ���� �԰� �ڸ� �� �����ִ��� Ȯ��
                
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�
                SceneManager.LoadScene("JDH3");
                break;

            case PLACE.STAIRS_ELEVATOR:
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�
                SceneManager.LoadScene("JDH4");
                break;

            case PLACE.OUTSIDE:
                //������ ��縸 ��� �� ����
                break;

        }
    }
}
