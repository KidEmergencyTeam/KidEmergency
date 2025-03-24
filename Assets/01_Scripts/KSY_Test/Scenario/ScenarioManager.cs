using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.XR.Interaction.Toolkit; -> 보류된 손수건 상호작용 

// 빌드 세팅 목록
// 1. 00_Scenes/Tests/KSY/1.Lobby
// 2. 00_Scenes/Tests/KSY/2.School
// 3. 00_Scenes/Tests/KSY/3.Hallway
// 4. 00_Scenes/Tests/KSY/4.Elevator
// 5. 00_Scenes/Tests/KSY/5.Schoolyard
// 6. 00_Scenes/Tests/Fire_School_Ready
// 7. 00_Scenes/Tests/LJW/LJW_Start

// 파티클
[Serializable]
public class ParticleSetup
{
    [Header("연기 파티클")]
    public ParticleSystem smokeEffect;

    [Header("파티클 위치")]
    public List<Transform> particleSpawnPoints; 
}

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    [Header("스크립트")]
    public TypingEffect typingEffect;

    [Header("연기 파티클 + 위치")]
    public ParticleSetup smokeParticleData;

    [Header("씬 이름")]
    public List<string> sceneNames;

    // 현재 시나리오 스텝 번호
    private int currentStep = 1;
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;

    // 충돌 여부 플래그 (Step13에서 대기)
    // private bool handkerGrabbed = false;

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

        if (smokeParticleData == null)
        {
            Debug.LogError("ParticleSetup(연기 파티클 + 파티클 위치)가 설정되지 않았습니다.");
            return;
        }

        if (sceneNames == null || sceneNames.Count < 4)
        {
            Debug.LogError("Scene Names 리스트에 최소 4개 이상의 씬 이름이 필요합니다.");
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

        // FadeIn 효과 후 시나리오 실행
        StartCoroutine(StartSequence());
    }

    // FadeIn 효과가 완료된 후 시나리오를 실행하는 코루틴
    private IEnumerator StartSequence()
    {
        // 페이드 인 효과 실행 및 완료 대기
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());

        // 시나리오 실행 시작
        yield return StartCoroutine(RunScenario());
    }

    // 시나리오를 순차적으로 실행
    IEnumerator RunScenario()
    {
        // 딕셔너리에 등록된 전체 스텝 실행
        while (currentStep <= 38)
        {
            if (scenarioSteps.ContainsKey(currentStep))
            {
                // 현재 스텝의 코루틴을 실행하고 완료될 때까지 대기
                yield return StartCoroutine(scenarioSteps[currentStep]());
            }
            else
            {
                Debug.LogWarning($"[ScenarioManager] 구현되지 않은 스텝: {currentStep}");
            }

            // 다음 스텝으로 넘김 (없으면 같은 스텝에서 무한 반복됨)
            currentStep++;

            // 한 프레임 대기 (모든 로직 실행 후 다음 프레임에서 루프 재시작)
            yield return null;
        }
    }

    // 텍스트 출력과 사운드 재생이 모두 완료될 때까지 대기
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
        // 위치 반영해서 파티클 실행 
        if (smokeParticleData != null &&
            smokeParticleData.particleSpawnPoints != null &&
            smokeParticleData.particleSpawnPoints.Count > 0)
        {
            foreach (Transform spawnPoint in smokeParticleData.particleSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    ParticleSystem ps = Instantiate(smokeParticleData.smokeEffect, spawnPoint.position, Quaternion.identity);
                    ps.Play();
                }
            }
        }

        yield return null;
    }
    IEnumerator Step4() { yield return PlayAndWait(2); }
    IEnumerator Step5() { yield return PlayAndWait(3); }
    IEnumerator Step6() { yield return PlayAndWait(4); }

    // Step7 선택지: 손 vs 손수건
    // 손수건 오브젝트 상호작용 보류
    IEnumerator Step7()
    {
        int selected = 0;
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(0, (result) => {
            selected = result;
        }));

        // 정답: 손 선택 시 Step9로 이동
        if (selected == 1)
            currentStep = 8;
        // 오답: 손수건 선택 시 Step12로 이동
        else
            currentStep = 11;
    }
    IEnumerator Step8() { yield return null; }
    IEnumerator Step9() { yield return PlayAndWait(5); }

    // Step10 대사 출력 -> Step13 진행
    IEnumerator Step10()
    {
        yield return PlayAndWait(6);
        currentStep = 12;
    }
    IEnumerator Step11() { yield return null; }
    IEnumerator Step12() { yield return PlayAndWait(7); }
    IEnumerator Step13()
    {
        // 태그가 "NPC"인 오브젝트들을 모두 찾음
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in npcs)
        {
            // 각 오브젝트에서 NpcRig.cs 가져오기
            NpcRig npcRig = npc.GetComponent<NpcRig>();

            if (npcRig != null)
            {
                // state를 Hold로 설정
                npcRig.state = NpcRig.State.Hold;
            }
        }

        yield return null;

        // 아래 손수건 관련 로직 주석 처리
        /*
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

        // 출력된 대사가 바로 사라지지 않고,
        // 손수건 상호작용 완료할 때까지 화면에 유지
        // yield return null; -> 상호작용 기능 완료하면 다시 사용
        */
    }

    // Step14에서는 PlayerPosition.cs를 이용하여 플레이어를 각 슬롯의 스텝14 위치로 이동
    IEnumerator Step14()
    {
        // PlayerPosition.cs 가져오기
        PlayerPosition playerPosition = FindObjectOfType<PlayerPosition>();
        if (playerPosition == null)
        {
            Debug.LogError("PlayerPosition 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // playerEntries 리스트 유효성 검사
        if (playerPosition.playerEntries == null || playerPosition.playerEntries.Count == 0)
        {
            Debug.LogError("playerEntries 리스트가 비어있습니다.");
            yield break;
        }

        // 페이드 아웃 효과 실행
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());

        // 할당된 모든 플레이어를 스텝14 위치와 회전으로 이동
        playerPosition.ApplyStep14Positions();

        // 페이드 인 효과 실행
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());

        yield return null;
    }

    // Step15 -> 페이드 아웃 효과 필수
    IEnumerator Step15()
    {
        yield return PlayAndWait(9);
        yield return StartCoroutine(ChangeScene(0));
    }
    IEnumerator Step16() { yield return PlayAndWait(10); }
    IEnumerator Step17() { yield return PlayAndWait(11); }

    // Step18 화재 경보벨 연출 -> 여기서부터 화재 경보벨 사운드 출력
    // Step35까지 화재 경보벨 사운드 출력
    IEnumerator Step18()
    {
        TypingEffect.Instance.StartContinuousSeparateTypingClip();
        yield return null;
    }
    IEnumerator Step19() { yield return PlayAndWait(12); }
    IEnumerator Step20() { yield return PlayAndWait(13); }

    // Step21 대사 출력 이후
    // 피난 유도선 아웃라인 효과 실행 -> Emergency_Exit 오브젝트 대상
    IEnumerator Step21()
    {
        // 태그가 "SafetyLine"인 오브젝트들을 모두 찾음
        GameObject[] safetyLineObjects = GameObject.FindGameObjectsWithTag("SafetyLine");

        foreach (GameObject obj in safetyLineObjects)
        {
            // 각 오브젝트에서 ToggleOutlinable.cs 가져오기
            ToggleOutlinable toggleComp = obj.GetComponent<ToggleOutlinable>();

            if (toggleComp != null)
            {
                // Outlinable 활성화 (false에서 true로 전환)
                toggleComp.OutlinableEnabled = true;
            }
            else
            {
                Debug.LogError("오브젝트 '" + obj.name + "'에 ToggleOutlinable 컴포넌트가 존재하지 않습니다.");
            }
        }
        yield return PlayAndWait(14);
    }

    // Step22 유저가 몸을 숙이는 애니메이션을 보여준다 유저의 시점이 낮아진다.
    IEnumerator Step22()
    {
        // 태그가 "NPC"인 오브젝트들을 모두 찾음
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in npcs)
        {
            // 각 오브젝트에서 NpcRig.cs 가져오기
            NpcRig npcRig = npc.GetComponent<NpcRig>();

            if (npcRig != null)
            {
                // state를 Bow로 설정
                npcRig.state = NpcRig.State.Bow;
            }
        }

        yield return null;
    }

    IEnumerator Step23() { yield return PlayAndWait(15); }

    // Step24 선택지: 피난유도선 vs 익숙한 길 
    IEnumerator Step24()
    {
        int selected = 0;
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(1, r => {
            selected = r;
        }));

        // 정답: 피난 유도선 선택 시 Step25로 이동
        if (selected == 1)
            currentStep = 24;
        // 오답: 익숙한 길 선택 시 Step27로 이동
        else
            currentStep = 26;
    }

    // Step25 대사 출력 -> Step28 진행
    IEnumerator Step25()
    {
        yield return PlayAndWait(16);
        currentStep = 27;
    }
    IEnumerator Step26() { yield return null; }
    IEnumerator Step27() { yield return PlayAndWait(17); }

    // Step28 -> 페이드 아웃 효과 필수
    IEnumerator Step28()
    {
        yield return PlayAndWait(18);
        yield return StartCoroutine(ChangeScene(1));
    }
    IEnumerator Step29() { yield return PlayAndWait(19); }
    IEnumerator Step30() { yield return PlayAndWait(20); }

    // Step31 선택지: 계단 VS 엘베
    IEnumerator Step31()
    {
        int selected = 0;
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(2, r => {
            selected = r;
        }));

        // 정답: 계단 선택 시 Step32로 이동
        if (selected == 1)
            currentStep = 31;
        // 오답: 엘리베이터 선택 시 Step34로 이동
        else
            currentStep = 33;
    }

    // Step32 대사 출력 -> Step35 진행
    IEnumerator Step32()
    {
        yield return PlayAndWait(21);
        currentStep = 34;
    }
    IEnumerator Step33() { yield return null; }
    IEnumerator Step34() { yield return PlayAndWait(22); }

    // Step35까지 연기 파티클 유지
    // Step35 -> 페이드 아웃 효과 필수
    IEnumerator Step35() 
    {
        yield return PlayAndWait(23);
        yield return StartCoroutine(ChangeScene(2));
    }

    // Step36: 연속 재생 종료 후 타이핑 실행
    IEnumerator Step36()
    {
        TypingEffect.Instance.StopContinuousSeparateTypingClip();
        yield return PlayAndWait(24);
    }
    IEnumerator Step37() { yield return PlayAndWait(25); }
    IEnumerator Step38() 
    { 
        yield return PlayAndWait(26); 
        yield return StartCoroutine(ChangeScene(3)); 
    }

    #endregion

    // 비동기 방식으로 씬 전환
    IEnumerator ChangeScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            Debug.Log($"씬 전환: {sceneNames[sceneIndex]}");

            // 페이드 아웃 효과 실행
            yield return StartCoroutine(FadeInOut.Instance.FadeOut());

            // 씬 전환 
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNames[sceneIndex]);
            while (!asyncLoad.isDone)
            {
                // 로딩 대기
                yield return null;
            }

            // 씬 로드 후 페이드 인 효과 실행
            yield return StartCoroutine(FadeInOut.Instance.FadeIn());
        }
        else
        {
            Debug.LogError($"유효하지 않은 씬 인덱스: {sceneIndex}");
        }
    }

    // 비활성화된 손수건 오브젝트 검색 (주석 처리)
    /*
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
    */

    /*
    // MouthDetector로부터 손수건과 충돌이 감지되면 호출
    public void HandkerGrabbed()
    {
        Debug.Log("ScenarioManager: 손수건과 충돌이 감지");
        handkerGrabbed = true;
        // 추가 로직 구현 가능
    }
    */
}
