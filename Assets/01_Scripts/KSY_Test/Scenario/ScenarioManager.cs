using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    [Header("스크립트")]
    public TypingEffect typingEffect;

    [Header("연기 파티클")]
    public ParticleSystem smokeEffect;

    [Header("씬 이름")]
    public List<string> sceneNames;

    // 현재 시나리오 스텝 번호
    private int currentStep = 1;
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;

    // 충돌 여부 플래그 (Step13에서 대기)
    private bool handkerGrabbed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (typingEffect == null)
        {
            Debug.LogError("TypingEffect가 할당되지 않았습니다.");
            return;
        }
        if (sceneNames == null || sceneNames.Count < 3)
        {
            Debug.LogError("Scene Names 리스트에 최소 3개 이상의 씬 이름이 필요합니다.");
            return;
        }

        // 스텝별 시나리오 정의
        scenarioSteps = new Dictionary<int, Func<IEnumerator>>()
        {
            { 1, Step1 },
            { 2, Step2 },
            { 3, Step3 },
            { 4, Step4 },
            { 5, Step5 },
            { 6, Step6 },
            { 7, Step7 },
            { 8, Step8 },
            { 9, Step9 },
            { 10, Step10 },
            { 11, Step11 },
            { 12, Step12 },
            { 13, Step13 },
            { 14, Step14 },
            { 15, Step15 },
            { 16, Step16 },
            { 17, Step17 },
            { 18, Step18 },
            { 19, Step19 },
            { 20, Step20 },
            { 21, Step21 },
            { 22, Step22 },
            { 23, Step23 },
            { 24, Step24 },
            { 25, Step25 },
            { 26, Step26 },
            { 27, Step27 },
            { 28, Step28 },
            { 29, Step29 },
            { 30, Step30 },
            { 31, Step31 },
            { 32, Step32 },
            { 33, Step33 },
            { 34, Step34 },
            { 35, Step35 },
            { 36, Step36 },
            { 37, Step37 },
            { 38, Step38 }
        };

        // 시나리오 실행 시작
        StartCoroutine(RunScenario());
    }

    // 시나리오 전체를 순차적으로 실행
    IEnumerator RunScenario()
    {
        while (currentStep <= 38)
        {
            if (scenarioSteps.ContainsKey(currentStep))
            {
                yield return StartCoroutine(scenarioSteps[currentStep]());
            }
            else
            {
                Debug.LogWarning($"[ScenarioManager] 구현되지 않은 스텝: {currentStep}");
            }
            currentStep++;
            yield return null;
        }
    }

    // 대사 출력 및 타이핑 효과 완료까지 대기
    public IEnumerator PlayAndWait(int index)
    {
        typingEffect.PlayTypingAtIndex(index);
        yield return new WaitUntil(() => !typingEffect.IsTyping);
    }

    #region 시나리오 스텝

    IEnumerator Step1() { yield return PlayAndWait(0); }
    IEnumerator Step2() { yield return PlayAndWait(1); }
    IEnumerator Step3()
    {
        if (smokeEffect != null)
        {
            var ps = Instantiate(smokeEffect);
            ps.Play();
        }
        yield return null;
    }
    IEnumerator Step4() { yield return PlayAndWait(2); }
    IEnumerator Step5() { yield return PlayAndWait(3); }
    IEnumerator Step6() { yield return PlayAndWait(4); }

    // 7. 손(1) vs 손수건(2)
    IEnumerator Step7()
    {
        int selected = 0;
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(0, (result) => {
            selected = result;
        }));

        if (selected == 1)
            currentStep = 8 - 1;
        else
            currentStep = 11 - 1;
    }
    IEnumerator Step8() { yield return null; }
    IEnumerator Step9() { yield return PlayAndWait(5); }
    IEnumerator Step10()
    {
        yield return PlayAndWait(6);
        currentStep = 13 - 1;
    }
    IEnumerator Step11() { yield return null; }
    IEnumerator Step12()
    {
        yield return PlayAndWait(7);
        currentStep = 13 - 1;
    }
    IEnumerator Step13()
    {
        yield return PlayAndWait(8);

        // 비활성화된 손수건 오브젝트도 찾기
        GameObject handker = FindInactiveObjectWithTag("Handker");
        if (handker != null)
        {
            // 손수건 활성화
            handker.SetActive(true);
            Debug.Log("손수건 오브젝트가 활성화되었습니다.");
        }
        else
        {
            Debug.LogError("손수건 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        // XRGrabInteractable 컴포넌트 가져오기
        XRGrabInteractable grab = handker.GetComponent<XRGrabInteractable>();
        if (grab == null)
        {
            Debug.LogError("손수건 오브젝트에 XRGrabInteractable 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 손수건 충돌 여부 플래그 초기화
        handkerGrabbed = false;
        Debug.Log("손수건을 잡고 입에 가져다주세요");

        // HandkerGrabbed()가 호출될 때까지 대기
        yield return new WaitUntil(() => handkerGrabbed);
        Debug.Log("손수건과 충돌이 감지되어 다음 스텝으로 진행합니다.");
    }
    IEnumerator Step14()
    {
        // 예시: 플레이어 이동 처리 등 추가 로직 구현 가능
        PlayerPosition playerPosition = FindObjectOfType<PlayerPosition>();
        if (playerPosition == null)
        {
            Debug.LogError("PlayerPosition 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }
        if (playerPosition.destinationPositions == null || playerPosition.destinationPositions.Count == 0)
        {
            Debug.LogError("destinationPositions 리스트가 비어있습니다.");
            yield break;
        }
        Destination destination = playerPosition.destinationPositions[0];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다.");
            yield break;
        }
        yield return StartCoroutine(MovePlayerToDestination(player, destination));
        yield return null;
    }
    IEnumerator Step15()
    {
        yield return PlayAndWait(9);
        yield return StartCoroutine(ChangeScene(0));
    }
    IEnumerator Step16() { yield return PlayAndWait(10); }
    IEnumerator Step17() { yield return PlayAndWait(11); }
    IEnumerator Step18() { yield return null; }
    IEnumerator Step19() { yield return PlayAndWait(12); }
    IEnumerator Step20() { yield return PlayAndWait(13); }
    IEnumerator Step21() { yield return PlayAndWait(14); }
    IEnumerator Step22() { yield return null; }
    IEnumerator Step23() { yield return PlayAndWait(15); }
    // 24. 피난유도선(1) vs 익숙한 길(2)
    IEnumerator Step24()
    {
        int selected = 0;
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(1, r => {
            selected = r;
        }));

        if (selected == 1)
            currentStep = 25 - 1;
        else
            currentStep = 26 - 1;
    }
    IEnumerator Step25()
    {
        yield return PlayAndWait(16);
        currentStep = 28 - 1;
    }
    IEnumerator Step26() { yield return null; }
    IEnumerator Step27()
    {
        yield return PlayAndWait(17);
        currentStep = 28 - 1;
    }
    IEnumerator Step28()
    {
        yield return PlayAndWait(18);
        yield return StartCoroutine(ChangeScene(1));
    }
    IEnumerator Step29() { yield return PlayAndWait(19); }
    IEnumerator Step30() { yield return PlayAndWait(20); }
    IEnumerator Step31() { yield return null; }
    IEnumerator Step32() { yield return PlayAndWait(21); currentStep = 35 - 1; }
    IEnumerator Step33() { yield return null; }
    IEnumerator Step34() { yield return PlayAndWait(22); currentStep = 35 - 1; }
    IEnumerator Step35()
    {
        yield return PlayAndWait(23);
        yield return StartCoroutine(ChangeScene(2));
        yield return null;
    }
    IEnumerator Step36() { yield return PlayAndWait(24); }
    IEnumerator Step37() { yield return PlayAndWait(25); }
    IEnumerator Step38() { yield return PlayAndWait(26); }

    #endregion

    // 비동기 씬 전환 메서드
    IEnumerator ChangeScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            Debug.Log($"씬 전환: {sceneNames[sceneIndex]}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNames[sceneIndex]);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError($"유효하지 않은 씬 인덱스: {sceneIndex}");
        }
    }

    // 플레이어를 목적지로 부드럽게 이동 (위치 및 회전 적용)
    IEnumerator MovePlayerToDestination(GameObject player, Destination destination)
    {
        float speed = 3f;
        while (Vector3.Distance(player.transform.position, destination.position) > 0.1f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, destination.position, speed * Time.deltaTime);
            yield return null;
        }
        player.transform.position = destination.position;
        player.transform.rotation = Quaternion.Euler(destination.rotation);
    }

    // 비활성화된 오브젝트도 검색할 수 있는 커스텀 함수
    private GameObject FindInactiveObjectWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // 씬에 속해있는 오브젝트이고, 태그가 일치하는 경우 반환
            if (obj.CompareTag(tag))
            {
                return obj;
            }
        }
        return null;
    }

    // MouthDetector로부터 손수건과 충돌이 감지되면 호출
    public void HandkerGrabbed()
    {
        Debug.Log("ScenarioManager: 손수건과 충돌이 감지");
        handkerGrabbed = true;
        // 추가 로직 구현 가능
    }
}
