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

    [Header("��� ����")]
    public PLACE place;

    [Header("ȭ������ �������� Ȯ���ϴ� ����")]
    public bool isFireBeginner;
    public bool isEarthquake;

    [Header("NPC �� ��ȣ�ۿ� ������Ʈ")]
    public GameObject player;   //�̱� �÷��̾� ��ġ
    public GameObject seti;
    public GameObject[] NPC;    //��Ƽ �÷��̾� ��ġ
    public FadeInOut fadeInOutImg;
    public TestButton2 okBtn;
    public GameObject exampleDescUi;
    public GameObject leftHand; //�޼� ������Ʈ
    [SerializeField] private GameObject emergencyExit;    //�ڽ� ������Ʈ�� addcomponent�� ���� Outlinable�� ����
    [SerializeField] private GameObject handkerchief;
    [SerializeField] private GameObject fireAlarm;

    [Header("�Ӹ� ���� ���� ����")]
    public bool isHeadDown = false;
    public float headHeightThreshold; // ���� ���� (�� ������ �������� ���� ������ �Ǵ�)

    [SerializeField] private Transform xrCamera;  // HMD ī�޶� Ʈ��ŷ
    [SerializeField]private float initialHeight; // �ʱ� �÷��̾� ���� ����

    [Header("��ȣ�ۿ� Ȥ�� ��ġ �̵� ����")]
    public Transform playerMovPos;
    public Transform setiMovPos;
    public Transform[] npcMovPos;

    [Header("����ϴ� ��ƼŬ")]
    public ParticleSystem smokeParticle;

    [Header("ExampleUI ���� �̹��� ���� ����")]
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private Sprite leftChangeImg;
    [SerializeField] private Sprite rightChangeImg;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("�����Ȳ üũ ����")]
    public bool isFirstStepRdy;
    public bool isSecondStepRdy;
    public bool hasHandkerchief;
    public bool iscoverFace;

    [Header("�ó����� ��� ���")]
    [SerializeField] private BeginnerDialogSystem firstDialog;
    [SerializeField] private BeginnerDialogSystem secondDialog;
    [SerializeField] private BeginnerDialogSystem thirdDialog;

    private void Awake()
    {
        xrCamera = Camera.main.transform;
        initialHeight = xrCamera.position.y; // ���� ���� �� �ʱ� ���� ����
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        switch (place)
        {
            //����
            case PLACE.CLASSROOM:
                fadeInOutImg.gameObject.SetActive(false);
                //fadeInOutImg.StartCoroutine(fadeInOutImg.FadeIn());
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                //ȭ�� �溸���� �Բ� �ι�° �ó����� ��� ���
                secondDialog.gameObject.SetActive(true);
                //ȭ�� �溸�� ���
                fireAlarm.SetActive(true);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                //��� ���� �� ��ư Ȱ��ȭ, ��ư ������ ������ ���
                okBtn.gameObject.SetActive(true);

                yield return new WaitUntil(() => okBtn.isHovered == true);
                //���� ������ isFirstStepRdy �� true�� �� ���� ����Ѵ�. (�̹��� ����)
                LeftImg.sprite = leftChangeImg;
                RightImg.sprite = rightChangeImg;
                isFirstStepRdy = true;

                yield return new WaitUntil(() => isFirstStepRdy == true);
                okBtn.gameObject.SetActive(false);
                exampleDescUi.SetActive(true);
                //å�� �ռ��� ����
                handkerchief.gameObject.SetActive(true);
                //������ �ռ��� ������ �޼տ� ���� (������ �� ���� ���)
                yield return new WaitUntil(() => hasHandkerchief == true);
                Debug.Log("�ռ��� ���� �Ϸ�");
                //���� �� �ֺ��� �ռ��� ���� �� �԰� �ڸ� ���� ������ ����

                yield return new WaitUntil(() => iscoverFace == true);
                //FadeIn, Out���� �̵��ϴ� ����� �Ⱥ����ش�.
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                // �÷��̾�� NPC�� ��ġ�� �� ������ �̵�, ��Ƽ ���� ��ġ ����
                Debug.Log("�÷��̾� NPC ��ġ �̵�");
                TeleportCharacters();
                StartCoroutine(fadeInOutImg.FadeIn());

                //�÷��̾�� NPC�� �̵��ϰ� �԰� �ڸ� ���� ������ �����Ǹ�
                isSecondStepRdy = true;
                //isSecondStepRdy�� true�� �Ǹ� ����° �ó����� ��� ���
                yield return new WaitUntil(() => isSecondStepRdy == true);
                thirdDialog.gameObject.SetActive(true);
                fadeInOutImg.gameObject.SetActive(true);
                //Fade Out ���� �� �� Scene �̵�
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�
                SceneManager.LoadScene("JDH2");
                break;

            //����
            case PLACE.HALLWAY:
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);
                secondDialog.gameObject.SetActive(true);
                //�ǳ� ������ �׵θ� ����
                ActiveOutlineToChildren(emergencyExit);
                yield return new WaitUntil(() => secondDialog.isDialogsEnd == true);
                thirdDialog.gameObject.SetActive(true);
                //�÷��̾ �ռ����� ���� �԰� �ڸ� �� �����ִ��� Ȯ��(��� UI ���: �ռ������� �԰� �ڸ� ������!)


                //Fade Out ���� �� �� Scene �̵�
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�
                SceneManager.LoadScene("JDH3");
                break;

            //���, ����������
            case PLACE.STAIRS_ELEVATOR:
                //��� ������ �Ϸ�Ǿ��⿡ ��ư Ŭ�� �� ���� ������ �̵�(��� UI ���: �ռ������� �԰� �ڸ� ������!)
                yield return new WaitUntil(() => firstDialog.isDialogsEnd == true);

                //��ư Ŭ�� ���

                //Fade Out ���� �� �� Scene �̵�
                StartCoroutine(fadeInOutImg.FadeOut());
                yield return new WaitUntil(() => fadeInOutImg.isFadeOut == false);
                SceneManager.LoadScene("JDH4");
                break;

            //��ġ�� �� ������
            case PLACE.OUTSIDE:
                //������ ��縸 ��� �� ����
                firstDialog.gameObject.SetActive(true);
                break;

        }
    }
    private void Update()
    {
        DetectHeadLowering(); // �Ӹ� ���� ���� �Լ� ȣ��
    }

    private void TeleportCharacters()
    {
        // �÷��̾� ��� �̵�
        if (playerMovPos != null)
        {
            player.transform.position = playerMovPos.position;
        }

        // ��Ƽ ��� �̵�
        if (setiMovPos != null)
        {
            seti.transform.position = setiMovPos.position;
        }

        // NPC ��� �̵�
        for (int i = 0; i < NPC.Length; i++)
        {
            if (npcMovPos.Length > i && npcMovPos[i] != null)
            {
                NPC[i].transform.position = npcMovPos[i].position;
            }
        }

        Debug.Log("�÷��̾� �� NPC �ڷ���Ʈ �Ϸ�");
    }

    //�ڽ� ������Ʈ�� Outlinable�� Ȱ��ȭ
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
    private void DetectHeadLowering()
    {
        if (xrCamera == null) return;

        float currentHeight = xrCamera.position.y; // ���� �Ӹ� ����

        // �Ӹ� ���̰� ���غ��� �������� ���� ������ �Ǵ�
        if (currentHeight < headHeightThreshold)
        {
            isHeadDown = true;
            Debug.Log("�Ӹ��� �������ϴ�! (Y�� ����)");
        }
        else
        {
            isHeadDown = false;
        }
    }

}
