using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // 일정 시간 이후 시나리오 실행
        StartCoroutine(DelayedScenarioCall(2f));
    }

    // 일정 시간 이후 시나리오 실행
    private IEnumerator DelayedScenarioCall(float delay)
    {
        // delay 만큼 대기
        yield return new WaitForSeconds(delay);

        // 시나리오 실행
        StartCoroutine(RunScenario());
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

            yield return null;
        }
    }

    // 텍스트 출력과 사운드 재생이 모두 완료될 때까지 대기
    public IEnumerator PlayAndWait(int index)
    {
        // 만약에 세티 표정 변화를 제대로 반영하겠다 싶으면 
        // 여기서 처리하면 된다.
        typingEffect.PlayTypingAtIndex(index);
        yield return new WaitUntil(() => !typingEffect.IsTyping);
    }

    #region 시나리오 스텝

    IEnumerator Step1() 
    {
        // DialogUI 활성화
        yield return StartCoroutine(DialogUIActivation());
        yield return PlayAndWait(0); 
    }

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
        // 패널에서 설정한 정답
        int selected = 0;

        // 선택지 정답 -> 두 값을 비교해서 일치하면 세티 표정을 변경하기 위해 2개의 값이 존재
        // 만약 값이 1개만 존재하면 정답이 1일 수도 있고, 2일 수도 있는데
        // 이것을 구분하여 SetHappy을 호출하기 어렵다.
        int correct = 2;

        yield return StartCoroutine(
            ChoiceVoteManager.Instance.ShowChoiceAndGetResult(0, result => selected = result)
        );

        // 일치 -> 정답, 스텝 12 이동
        if (selected == correct)
            currentStep = 11;

        // 불일치 -> 오답, 스텝 9 이동
        else
            currentStep = 8;

        StartCoroutine(ApplySelectionAndDelayedReset(3f, selected, correct));
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
        yield return PlayAndWait(8);

        // 비활성화된 손수건 활성화
        yield return StartCoroutine(Handker());

        // NPC들의 상태를 Hold로 변경
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Hold));

        // 손수건 충돌 확인 -> 확인되면 다음 스텝으로 넘어감
        yield return StartCoroutine(Mask());
    }

    // Step14에서 페이드 효과 + 문 앞으로 이동
    IEnumerator Step14()
    {
        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃 효과 실행
        yield return StartCoroutine(OVRScreenFade.Instance.Fade(0, 1));

        // 문 앞으로 이동
        yield return StartCoroutine(Positions());

        // 페이드 인 효과 실행
        yield return StartCoroutine(OVRScreenFade.Instance.Fade(1, 0));
    }

    // Step15 -> 페이드 아웃 효과 필수
    IEnumerator Step15()
    {
        yield return new WaitForSeconds(0.5f);
        yield return PlayAndWait(9);
        yield return StartCoroutine(ChangeScene(0));
    }

    // Step16: 씬 전환 후 NPC 상태를 Hold로 변경하고 대사 출력
    IEnumerator Step16()
    {
        yield return StartCoroutine(PlaySmokeParticles());
        yield return StartCoroutine(SetAllNPCsState(NpcRig.State.Hold));
        yield return new WaitForSeconds(0.5f);

        // DialogUI 활성화
        yield return StartCoroutine(DialogUIActivation());
        yield return PlayAndWait(10);
    }

    IEnumerator Step17() { yield return PlayAndWait(11); }

    // Step18: 화재 경보벨 연출 -> 버튼 클릭 대기 후 화재 경보벨 재생
    // Step35까지 화재 경보벨 사운드 출력
    // 코루틴 처리 안한 이유: 중단 없이 전 스텝에 출력하기 위해
    IEnumerator Step18()
    {
        // 화재 경보벨 누름
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

        Debug.Log("비상벨을 누름");

        // 화재 경보벨 재생
        TypingEffect.Instance.StartContinuousSeparateTypingClip();
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
        //// 태그가 "SafetyLine"인 오브젝트들을 모두 찾음
        //GameObject[] safetyLineObjects = GameObject.FindGameObjectsWithTag("SafetyLine");

        //foreach (GameObject obj in safetyLineObjects)
        //{
        //    // 각 오브젝트에서 ToggleOutlinable.cs 가져오기
        //    ToggleOutlinable toggleComp = obj.GetComponent<ToggleOutlinable>();

        //    if (toggleComp != null)
        //    {
        //        // Outlinable 활성화 (false에서 true로 전환)
        //        toggleComp.OutlinableEnabled = true;
        //    }
        //    else
        //    {
        //        Debug.LogError("오브젝트 '" + obj.name + "'에 ToggleOutlinable 컴포넌트가 존재하지 않습니다.");
        //    }
        //}
        yield return PlayAndWait(14);
    }

    // Step22: NPC 상태를 Bow로 변경 -> 운동장 씬 전까지 유지
    // 사용자 낮은 자세로 숙이기
    IEnumerator Step22()
    {
        yield return StartCoroutine(VRCamera());
    }

    IEnumerator Step23() { yield return PlayAndWait(15); }

    // Step24: 선택지 처리 (피난유도선 vs 익숙한 길)
    IEnumerator Step24()
    {
        // 패널에서 설정한 정답
        int selected = 0;

        // 선택지 정답
        int correct = 1;

        yield return StartCoroutine(
            ChoiceVoteManager.Instance.ShowChoiceAndGetResult(1, result => selected = result)
        );

        // 일치 -> 정답, 스텝 25 이동
        if (selected == correct)
            currentStep = 24;

        // 불일치 -> 오답, 스텝 27 이동
        else
            currentStep = 26;

        StartCoroutine(ApplySelectionAndDelayedReset(3f, selected, correct));
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
        yield return new WaitForSeconds(0.5f);

        // DialogUI 활성화
        yield return StartCoroutine(DialogUIActivation());
        yield return PlayAndWait(19);
    }

    IEnumerator Step30() { yield return PlayAndWait(20); }

    // Step31: 선택지 처리 (계단 VS 엘베)
    IEnumerator Step31()
    {
        // 패널에서 설정한 정답
        int selected = 0;

        // 선택지 정답
        int correct = 1;

        yield return StartCoroutine(
            ChoiceVoteManager.Instance.ShowChoiceAndGetResult(2, result => selected = result)
        );

        // 일치 -> 정답, 스텝 32 이동
        if (selected == correct)
            currentStep = 31;

        // 불일치 -> 오답, 스텝 34 이동
        else
            currentStep = 33;

        StartCoroutine(ApplySelectionAndDelayedReset(3f, selected, correct));
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
        // 씬 이동 이후 -> 손수건 제거
        GrabStatePersistence.Instance.disableSingleton = true;
        TypingEffect.Instance.StopContinuousSeparateTypingClip();
        yield return new WaitForSeconds(0.5f);

        // DialogUI 활성화
        yield return StartCoroutine(DialogUIActivation());
        yield return PlayAndWait(24);
    }

    IEnumerator Step37() { yield return PlayAndWait(25); }

    // 마지막 대사 출력 -> 마지막 씬 이동
    IEnumerator Step38()
    {
        yield return PlayAndWait(26);
        yield return StartCoroutine(SetRobotState(3f));
        yield return StartCoroutine(ChangeScene(3));

        // 마지막 씬 이동 이후 -> 싱글톤 매니저를 상속받는 객체 개별적으로 Destroy
        ChoiceVoteManager.Instance.disableSingleton = true;
        TypingEffect.Instance.disableSingleton = true;

        // 시나리오 매니저 인스터스 제거
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

            // 씬 전환 
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNames[sceneIndex]);

            while (!asyncLoad.isDone)
            {
                // 로딩 대기
                yield return null;
            }

            // 씬 로드 후 페이드 인 효과 실행
            yield return StartCoroutine(OVRScreenFade.Instance.Fade(1, 0));
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

    // 문 앞으로 이동
    public IEnumerator Positions()
    {
        // PlayerPosition 컴포넌트 가져오기
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

        // 할당된 모든 플레이어를 스텝14 위치와 회전으로 이동
        playerPosition.ApplyStep14Positions();

        Debug.Log("문 앞으로 이동");
    }

    // 비활성화된 손수건 활성화
    public IEnumerator Handker()
    {
        // 1."HandkerActivation" 태그를 가진 객체에서 HandkerActivation.cs 가져오기
        HandkerActivation handkerActivation = GameObject.FindGameObjectWithTag("HandkerActivation")?.GetComponent<HandkerActivation>();

        if (handkerActivation == null)
        {
            Debug.LogError("HandkerActivation 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 2.손수건 활성화
        handkerActivation.ActivateHandkerObject();

        Debug.Log("손수건 활성화");
    }

    // 손수건 충돌 확인
    public IEnumerator Mask()
    {
        // 1."Head" 태그를 가진 객체에서 FireEvacuationMask 컴포넌트 가져오기
        FireEvacuationMask fireEvacuationMask = GameObject.FindGameObjectWithTag("Head")?.GetComponent<FireEvacuationMask>();
        if (fireEvacuationMask == null)
        {
            Debug.LogError("FireEvacuationMask 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 2.충돌 체크를 위한 변수
        bool collisionOccurred = false;

        // 3.충돌이 발생하면 collisionOccurred를 true로 변경
        void OnHandkerEnterHandler()
        {
            collisionOccurred = true;
        }

        // 4.손수건과 충돌할 때 OnHandkerEnterHandler 실행
        fireEvacuationMask.OnHandkerchiefEnter += OnHandkerEnterHandler;

        // 5.충돌이 발생할 때까지 대기 
        yield return new WaitUntil(() => collisionOccurred);
        Debug.Log("손수건과 입 콜라이더가 충돌 - 다음 단계 실행");

        // 6.이벤트 해제
        fireEvacuationMask.OnHandkerchiefEnter -= OnHandkerEnterHandler;
    }

    // 플레이어 높낮이 체크
    public IEnumerator VRCamera()
    {
        // 1."MainCamera" 태그가 붙은 오브젝트를 찾습니다.
        GameObject vrCameraObj = GameObject.FindGameObjectWithTag("MainCamera");

        if (vrCameraObj == null)
        {
            Debug.LogError("MainCamera 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 높이가  -0.3 이상이면서 -0.2 이하인 범위에 도달할 때까지 대기
        yield return new WaitUntil(() =>

    vrCameraObj.transform.localPosition.y >= -0.3f &&

    vrCameraObj.transform.localPosition.y <= -0.2f);

        Debug.Log("높이가  -0.3 이상이면서 -0.2 이하인 범위에 도달하여 다음 스텝으로 진행합니다.");
    }

    // 세티 표정 초기화
    private IEnumerator ApplySelectionAndDelayedReset(float delay, int panelChoice, int choiceResult)
    {
        // "Seti" 태그가 붙은 오브젝트 찾기
        RobotController robotController = GameObject.FindGameObjectWithTag("Seti")?.GetComponent<RobotController>();
        if (robotController == null)
        {
            Debug.LogError("RobotController 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 정답이면 SetHappy 호출
        if (panelChoice == choiceResult)
        {
            robotController.SetHappy();
        }
        // 오답이면 SetAngry 호출
        else
        {
            robotController.SetAngry();
        }

        // 딜레이 적용
        yield return new WaitForSeconds(delay);

        // 세티 표정 복구
        robotController?.SetBasic();
    }

    // 모든 시나리오를 마친 후 세티 표정 SetHappy 반영
    private IEnumerator SetRobotState(float delay)
    {
        // "Seti" 태그가 붙은 오브젝트 찾기
        RobotController robotController = GameObject.FindGameObjectWithTag("Seti")?.GetComponent<RobotController>();
        if (robotController == null)
        {
            Debug.LogError("RobotController 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        // 세티 표정 SetHappy 반영
        robotController.SetHappy();

        // 딜레이 적용
        yield return new WaitForSeconds(delay);
    }

    // DialogUI 활성화
    private IEnumerator DialogUIActivation()
    {
        // "DialogUI" 태그가 붙은 오브젝트 찾기
        DialogUI dialogUI = GameObject.FindGameObjectWithTag("DialogUI")?.GetComponent<DialogUI>();
        if (dialogUI == null)
        {
            Debug.LogError("DialogUI 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }
        else
        {
            dialogUI.dialogPanel.SetActive(true);
        }
    }
}