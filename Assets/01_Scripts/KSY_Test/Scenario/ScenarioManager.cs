using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    // typingEffect.cs에서 대사 출력 관리
    [Header("스크립트")]
    public TypingEffect typingEffect; 

    [Header("연기 파티클")]
    public ParticleSystem smokeEffect;

    [Header("씬 이름")]
    public List<string> sceneNames; 

    // 현재 시나리오 스텝을 나타냄
    private int currentStep = 1;
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;

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
            { 37, Step37 }
        };

        // 시나리오 시작
        StartCoroutine(RunScenario());
    }

    // 시나리오 전체를 순차 실행
    IEnumerator RunScenario()
    {
        while (currentStep <= 37)
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

    // 대사 출력 + 타이핑 끝날 때까지 대기
    IEnumerator PlayAndWait(int index)
    {
        typingEffect.PlayTypingAtIndex(index);
        yield return new WaitUntil(() => !typingEffect.IsTyping);
    }

    #region 시나리오 스텝 예시

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
        // ChoiceVoteManager에서 0번 패널(예: 손 vs 손수건) 투표
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(0, (result) => {
            selected = result;
        }));

        if (selected == 1)
            currentStep = 8 - 1; // 다음 loop에서 currentStep++ => 8
        else
            currentStep = 11 - 1; // currentStep = 10; 형태가 아닌 currentStep = 11 - 1; 형태로 처리한 이유: 가독성
    }

    IEnumerator Step8() { yield return null; } // 선택지 1번 선택
    IEnumerator Step9() { yield return PlayAndWait(5); }
    IEnumerator Step10()
    {
        yield return PlayAndWait(6);
        currentStep = 13 - 1; // 현재 스텝을 12로 정정 -> 따라서 6번 대사 이후 스텝 13번부터 시작
    }
    IEnumerator Step11() { yield return null; } // 선택지 2번 선택
    IEnumerator Step12()
    {
        yield return PlayAndWait(7);
        currentStep = 13 - 1;
    }
    IEnumerator Step13() { yield return PlayAndWait(8); } 
    IEnumerator Step14() { yield return null; } // 여기서 2줄 정렬
    IEnumerator Step15()
    {
        yield return PlayAndWait(9);
        ChangeScene(0); // 교실 -> 복도 (sceneNames[0])
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
        ChangeScene(1); // 복도 -> 계단/엘리베이터 (sceneNames[1])
    }
    IEnumerator Step29() { yield return PlayAndWait(19); }
    IEnumerator Step30()
    {
        yield return PlayAndWait(20);

        int selected = 0;
        // 2번 패널(예: 계단 vs 엘리베이터)
        yield return StartCoroutine(ChoiceVoteManager.Instance.ShowChoiceAndGetResult(2, r => {
            selected = r;
        }));

        if (selected == 1)
            currentStep = 31 - 1;
        else
            currentStep = 33 - 1;
    }
    IEnumerator Step31() { yield return null; }
    IEnumerator Step32()
    {
        yield return PlayAndWait(21);
        currentStep = 35 - 1;
    }
    IEnumerator Step33() { yield return null; }
    IEnumerator Step34()
    {
        yield return PlayAndWait(22);
        currentStep = 35 - 1;
    }
    IEnumerator Step35()
    {
        yield return PlayAndWait(23);
        ChangeScene(2); // 계단/엘리베이터 -> 운동장 (sceneNames[2])
    }
    IEnumerator Step36() { yield return PlayAndWait(24); }
    IEnumerator Step37() { yield return PlayAndWait(25); }

    #endregion

    // 씬 전환 함수
    void ChangeScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            Debug.Log($"씬 전환: {sceneNames[sceneIndex]}");
            SceneManager.LoadScene(sceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogError($"유효하지 않은 씬 인덱스: {sceneIndex}");
        }
    }
}