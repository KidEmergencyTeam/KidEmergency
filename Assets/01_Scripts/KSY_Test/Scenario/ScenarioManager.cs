using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 빌드 세팅 목록
// 1. 00_Scenes/Tests/KSY/1.Lobby
// 2. 00_Scenes/Tests/KSY/2.School
// 3. 00_Scenes/Tests/KSY/3.Hallway
// 4. 00_Scenes/Tests/KSY/4.Elevator
// 5. 00_Scenes/Tests/KSY/5.Schoolyard
// 6. 00_Scenes/Tests/Fire_School_Ready
// 7. 00_Scenes/Tests/LJW/LJW_Start

// 씬별 파티클 설정
[Serializable]
public class ParticleData
{
    [Header("파티클 위치")]
    public Vector3 position;

    [Header("파티클 회전")]
    public Vector3 rotation;
}

[Serializable]
public class SceneParticleSetup
{
    [Header("씬 이름")]
    public string sceneName;

    [Header("연기 파티클")]
    public ParticleSystem smokeEffect;

    [Header("파티클 설정")]
    public List<ParticleData> particleData;
}

public class ScenarioManager : DisableableSingleton<ScenarioManager>
{
    [Header("스크립트")]
    public TypingEffect typingEffect;

    [Header("씬별 파티클 설정")]
    public List<SceneParticleSetup> sceneParticleSetups;

    [Header("씬 이름")]
    public List<string> sceneNames;

    // 생성된 모든 연기 파티클 저장
    private List<ParticleSystem> activeSmokeParticles = new List<ParticleSystem>();

    // 현재 시나리오 스텝 번호
    private int currentStep = 1;
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;

    private void Start()
    {
        if (typingEffect == null)
        {
            Debug.LogError("TypingEffect가 할당되지 않았습니다.");
            return;
        }

        // 씬별 파티클 설정 유효성 검사
        if (sceneParticleSetups == null || sceneParticleSetups.Count == 0)
        {
            Debug.LogError("씬별 파티클 설정이 정의되지 않았습니다.");
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

    // FadeIn 효과가 완료된 후 시나리오를 실행
    private IEnumerator StartSequence()
    {
        // 씬 로드 여부 체크
        bool sceneLoaded = false;

        // 씬 로드 완료 이벤트
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 이벤트 호출 시 처리 내용
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬이 로드 -> OnSceneLoaded 함수 호출
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 이미 씬이 로드된 상태라면 바로 처리
        if (SceneManager.GetActiveScene().isLoaded)
        {
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬 로드 완료까지 대기
        yield return new WaitUntil(() => sceneLoaded);

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

    // 교실 씬 -> 연기 파티클 처리
    IEnumerator Step3()
    {
        yield return StartCoroutine(PlaySmokeParticles());
    }
    IEnumerator Step4() { yield return PlayAndWait(2); }
    IEnumerator Step5() { yield return PlayAndWait(3); }
    IEnumerator Step6() { yield return PlayAndWait(4); }

    // Step7 선택지: 손 vs 손수건
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

    // Step13: NPC 상태를 Hold로 변경
    // 사용자 손수건 처리
    IEnumerator Step13()
    {
        // 대사 출력 -> 모두 책상 위에 있는 손수건을 사용해서 입과 코를 가리고 교실 문 앞에 2 줄로 서 줘! 
        yield return PlayAndWait(8);
        // 1. "HandkerActivation" 태그를 가진 객체에서 HandkerActivation.cs 가져오기
        HandkerActivation handkerActivation = GameObject.FindGameObjectWithTag("HandkerActivation")?.GetComponent<HandkerActivation>();
        // 2. 손수건 활성화
        handkerActivation.ActivateHandkerObject();
        // 3. NPC들의 상태를 Hold로 변경
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Hold));
        // 4. "Head" 태그를 가진 객체에서 FireEvacuationMask.cs 가져오기
        FireEvacuationMask fireEvacuationMask = GameObject.FindGameObjectWithTag("Head")?.GetComponent<FireEvacuationMask>();
        // 5. 충돌 체크 
        bool collisionOccurred = false;
        // 6. 충돌이 발생하면 collisionOccurred를 true로 변경
        fireEvacuationMask.OnHandkerchiefCollision += () => collisionOccurred = true;
        // 7. 충돌이 발생할 때까지 대기 
        yield return new WaitUntil(() => collisionOccurred);
        Debug.Log("손수건과 입 콜라이더가 충돌 - 다음 단계 실행");
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

    // Step16: 씬 전환 후 NPC 상태를 Hold로 변경하고 대사 출력
    IEnumerator Step16()
    {
        yield return StartCoroutine(PlaySmokeParticles());
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Hold));
        yield return PlayAndWait(10);
    }
    IEnumerator Step17() { yield return PlayAndWait(11); }

    // Step18: 화재 경보벨 연출 -> 버튼 클릭 대기 후 화재 경보벨 재생
    // Step35까지 화재 경보벨 사운드 출력
    IEnumerator Step18()
    {
        // 버튼 클릭할 때까지 대기
        bool buttonClicked = false;
        // 콜백 등록 
        Action callback = () => buttonClicked = true;
        // 버튼 클릭 이벤트에 콜백 등록 -> 버튼 클릭 시 이벤트 발생하면 콜백 실행 -> buttonClicked = true 처리
        EmergencyBellButton.OnEmergencyBellClicked += callback;
        // buttonClicked = true가 될 때까지 대기
        yield return new WaitUntil(() => buttonClicked);
        // 콜백 제거
        EmergencyBellButton.OnEmergencyBellClicked -= callback;
        // 화재 경보벨 재생
        TypingEffect.Instance.StartContinuousSeparateTypingClip();
        yield return null;
    }
    IEnumerator Step19() { yield return PlayAndWait(12); }

    // 연기 파티클 강조
    IEnumerator Step20() 
    {
        // 생성된 연기 파티클 y값 변경 
        yield return StartCoroutine(UpdateSmokeParticlesYCoroutine(2.0f));
        yield return PlayAndWait(13);
    }

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

    // Step22: NPC 상태를 Bow로 변경 -> 운동장 씬 전까지 유지
    // 사용자 낮은 자세로 숙이기
    IEnumerator Step22()
    {
        // 1. NPC들의 상태를 Bow(숙임)으로 변경
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Bow));

        // "MainCamera" 태그가 붙은 오브젝트를 찾습니다.
        GameObject vrCameraObj = GameObject.FindGameObjectWithTag("MainCamera");

        // 높이가  -0.3 이상이면서 -0.2 이하인 범위에 도달할 때까지 대기
        yield return new WaitUntil(() =>
    vrCameraObj.transform.localPosition.y >= -0.3f &&
    vrCameraObj.transform.localPosition.y <= -0.2f);

        Debug.Log("높이가  -0.3 이상이면서 -0.2 이하인 범위에 도달하여 다음 스텝으로 진행합니다.");
    }

    IEnumerator Step23() { yield return PlayAndWait(15); }

    // Step24: 선택지 처리 (피난유도선 vs 익숙한 길)
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

        // 씬 전환 -> NPC 상태 초기화
    }

    // Step29: 씬 전환 후 NPC 상태를 Bow로 변경하고 대사 출력
    IEnumerator Step29()
    {
        yield return StartCoroutine(PlaySmokeParticles());
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Bow));
        yield return PlayAndWait(19);
    }

    IEnumerator Step30() { yield return PlayAndWait(20); }

    // Step31: 선택지 처리 (계단 VS 엘베)
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

    // 마지막 대사 출력 -> 마지막 씬 이동
    IEnumerator Step38()
    {
        yield return PlayAndWait(26);
        yield return StartCoroutine(ChangeScene(3));

        // 마지막 씬 이동 이후 -> 싱글톤 매니저를 상속받는 객체 개별적으로 Destroy
        ChoiceVoteManager.Instance.disableSingleton = true;
        TypingEffect.Instance.disableSingleton = true;
        disableSingleton = true;
    }
    #endregion

    // 비동기 방식으로 씬 전환
    IEnumerator ChangeScene(int sceneIndex)
    {
        // 씬 이름 체크
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

    // NPC의 상태 전환
    private IEnumerator SetAllNPCsState(NpcRig.State desiredState)
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in npcs)
        {
            NpcRig npcRig = npc.GetComponent<NpcRig>();

            if (npcRig != null)
            {
                npcRig.state = desiredState;
            }
        }

        // 모든 NPC 한 번에 상태 전환 -> 많은 NPC가 있을 경우, 약간 버벅거릴 수 있음
        yield return null;
    }

    // 연기 파티클 실행
    public IEnumerator PlaySmokeParticles()
    {
        // 이전에 생성된 파티클 리스트 초기화
        activeSmokeParticles.Clear();

        // 현재 활성화된 씬 이름 확인
        string activeSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬에 해당하는 파티클 설정 찾기
        SceneParticleSetup particleSetup = sceneParticleSetups.Find(setup => setup.sceneName == activeSceneName);

        if (particleSetup != null && particleSetup.particleData != null && particleSetup.particleData.Count > 0)
        {
            foreach (ParticleData data in particleSetup.particleData)
            {
                Quaternion rotation = Quaternion.Euler(data.rotation);
                ParticleSystem ps = Instantiate(particleSetup.smokeEffect, data.position, rotation);
                ps.Play();

                // 생성된 파티클을 리스트에 저장
                activeSmokeParticles.Add(ps);
            }
        }
        else
        {
            Debug.LogWarning("해당 씬에 대한 파티클 설정이 없거나 파티클 데이터가 정의되지 않았습니다: " + activeSceneName);
        }

        yield return null;
    }

    // 생성된 연기 파티클 y값 변경 
    public IEnumerator UpdateSmokeParticlesYCoroutine(float newY)
    {
        foreach (ParticleSystem ps in activeSmokeParticles)
        {
            if (ps != null)
            {
                Vector3 pos = ps.transform.position;

                // y 값을 새 값으로 변경
                pos.y = newY; 
                ps.transform.position = pos;
            }
        }
        yield return null;
    }
}
