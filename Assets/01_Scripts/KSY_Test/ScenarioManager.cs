using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class ChoiceUIItem
{
    public GameObject panel;       // 선택지 패널 (하나의 묶음)
    public List<Button> buttons;   // 해당 패널에 포함된 선택 버튼들 (순서대로 1번, 2번, …)
}

public class ScenarioManager : MonoBehaviour
{
    // --------------------
    //   싱글톤 인스턴스
    // --------------------
    public static ScenarioManager Instance { get; private set; }

    // --------------------
    //   타이핑 효과 및 UI
    // --------------------
    public TypingEffect typingEffect;      // 타이핑 효과 스크립트
    public List<ChoiceUIItem> choiceUIItems; // 선택지 패널+버튼 묶음을 리스트로 처리

    // --------------------
    //   연기 파티클 관련
    // --------------------
    [Header("연기 파티클 (2번 대사 이후 재생)")]
    public ParticleSystem smokeEffect;

    // --------------------
    //      씬 전환
    // --------------------
    [Header("Scene Names (순서대로 할당)")]
    public List<string> sceneNames;        // 예: [0] 복도, [1] 계단/엘리베이터, [2] 운동장

    private int userChoice = 0;            // 유저가 선택한 값 (1,2,…)
    private int currentStep = 1;           // 현재 진행 스텝 (1~37)

    // 각 스텝을 매핑하는 딕셔너리
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;

    // 현재 활성화된 선택지 UI 인덱스 (리스트 내 인덱스)
    private int activeChoiceUIIndex = -1;

    // --------------------
    //  싱글톤 & DontDestroy
    // --------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);    // 이미 존재한다면, 새로 생긴 것은 제거
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
    }

    void Start()
    {
        // 필수 컴포넌트 체크
        if (typingEffect == null)
        {
            Debug.LogError("TypingEffect가 할당되지 않았습니다.");
            return;
        }
        if (choiceUIItems == null || choiceUIItems.Count == 0)
        {
            Debug.LogError("ChoiceUIItems 리스트에 하나 이상의 항목이 할당되어야 합니다.");
            return;
        }
        if (sceneNames == null || sceneNames.Count < 3)
        {
            Debug.LogError("씬 이름 리스트에 최소 3개 이상의 씬 이름이 할당되어야 합니다.");
            return;
        }

        // 각 ChoiceUIItem의 패널은 시작 시 모두 비활성화
        foreach (var item in choiceUIItems)
        {
            if (item.panel != null)
                item.panel.SetActive(false);
        }

        // 각 ChoiceUIItem의 버튼에 OnChoiceSelected 등록 (버튼 리스트 순서대로, 1부터)
        for (int i = 0; i < choiceUIItems.Count; i++)
        {
            // 각 패널에 포함된 버튼들에 대해 처리
            foreach (Button btn in choiceUIItems[i].buttons)
            {
                int choiceNumber = choiceUIItems[i].buttons.IndexOf(btn) + 1; // 1부터 시작
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnChoiceSelected(choiceNumber));
            }
        }

        // 시나리오 스텝들을 딕셔너리에 등록 (총 37 스텝)
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

    /// <summary>
    /// 전체 시나리오를 순차적으로 실행합니다.
    /// </summary>
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
                Debug.LogWarning("구현되지 않은 스텝: " + currentStep);
            }
            currentStep++;
            yield return null;
        }
    }

    /// <summary>
    /// index번째 텍스트를 타이핑 효과로 출력 후 타이핑이 끝날 때까지 대기합니다.
    /// </summary>
    IEnumerator PlayAndWait(int index)
    {
        typingEffect.PlayTypingAtIndex(index);
        yield return new WaitUntil(() => !typingEffect.IsTyping);
    }

    // -------------------------
    //       시나리오 스텝
    // -------------------------
    IEnumerator Step1()
    {
        // 첫 번째 대사 (인덱스 0)
        yield return PlayAndWait(0);
    }

    IEnumerator Step2()
    {
        // 두 번째 대사 (인덱스 1)
        yield return PlayAndWait(1);

        // === 여기서 연기 파티클 재생 ===
        if (smokeEffect != null)
        {
            ParticleSystem psInstance = Instantiate(smokeEffect);
            psInstance.Play();
        }

        // 파티클 재생 후 Step3를 건너뛰고 Step4로 점프
        currentStep = 4 - 1;  // 다음 while 루프에서 ++ 되어 4가 됨
    }

    // 아래 Step3는 실제로 스킵됨 (호출되지 않음)
    IEnumerator Step3()
    {
        yield return PlayAndWait(2);
    }

    IEnumerator Step4() { yield return PlayAndWait(3); }
    IEnumerator Step5() { yield return PlayAndWait(4); }
    IEnumerator Step6() { yield return PlayAndWait(5); }
    IEnumerator Step7()
    {
        ShowChoiceUI(0, true);
        yield return new WaitUntil(() => userChoice != 0);
        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(0, false);
        if (selected == 1)
            currentStep = 8 - 1;
        else
            currentStep = 11 - 1;
    }

    IEnumerator Step8()
    {
        yield return PlayAndWait(7);
        yield return StartCoroutine(Step9());
        yield return StartCoroutine(Step10());
    }

    IEnumerator Step9() { yield return PlayAndWait(8); }
    IEnumerator Step10() { yield return PlayAndWait(9); currentStep = 13 - 1; }
    IEnumerator Step11() { yield return PlayAndWait(10); yield return StartCoroutine(Step12()); }
    IEnumerator Step12() { yield return PlayAndWait(11); currentStep = 13 - 1; }

    IEnumerator Step13()
    {
        yield return PlayAndWait(12);
    }

    IEnumerator Step14()
    {
        yield return PlayAndWait(13);
    }

    IEnumerator Step15()
    {
        yield return PlayAndWait(14);
        // 씬 전환: 복도 (sceneNames[0])
        ChangeScene(0);
    }

    IEnumerator Step16() { yield return PlayAndWait(15); }
    IEnumerator Step17() { yield return PlayAndWait(16); }

    IEnumerator Step18()
    {
        yield return PlayAndWait(17);
    }

    IEnumerator Step19() { yield return PlayAndWait(18); }
    IEnumerator Step20() { yield return PlayAndWait(19); }
    IEnumerator Step21() { yield return PlayAndWait(20); }

    IEnumerator Step22()
    {
        yield return PlayAndWait(21);
    }

    IEnumerator Step23() { yield return PlayAndWait(22); }

    IEnumerator Step24()
    {
        yield return PlayAndWait(23);
        ShowChoiceUI(1, true);
        yield return new WaitUntil(() => userChoice != 0);
        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(1, false);
        if (selected == 1)
            currentStep = 25 - 1;
        else
            currentStep = 26 - 1;
    }

    IEnumerator Step25() { yield return PlayAndWait(24); currentStep = 28 - 1; }

    IEnumerator Step26()
    {
        yield return PlayAndWait(25);
        yield return StartCoroutine(Step27());
        currentStep = 28 - 1;
    }

    IEnumerator Step27() { yield return PlayAndWait(26); }

    IEnumerator Step28()
    {
        yield return PlayAndWait(27);
        // 씬 전환: 계단/엘리베이터 (sceneNames[1])
        ChangeScene(1);
    }

    IEnumerator Step29() { yield return PlayAndWait(28); }

    IEnumerator Step30()
    {
        yield return PlayAndWait(29);
        ShowChoiceUI(2, true);
        yield return new WaitUntil(() => userChoice != 0);
        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(2, false);
        if (selected == 1)
            currentStep = 31 - 1;
        else
            currentStep = 33 - 1;
    }

    IEnumerator Step31() { yield return PlayAndWait(30); yield return StartCoroutine(Step32()); }
    IEnumerator Step32() { yield return PlayAndWait(31); currentStep = 35 - 1; }
    IEnumerator Step33() { yield return PlayAndWait(32); yield return StartCoroutine(Step34()); }
    IEnumerator Step34() { yield return PlayAndWait(33); currentStep = 35 - 1; }

    IEnumerator Step35()
    {
        yield return PlayAndWait(34);
        // 씬 전환: 운동장 (sceneNames[2])
        ChangeScene(2);
    }

    IEnumerator Step36() { yield return PlayAndWait(35); }

    IEnumerator Step37()
    {
        yield return PlayAndWait(36);
        // TODO: 시나리오 종료 후 추가 연출
    }

    // -------------------------
    //  선택지 UI 관련 (ChoiceUIItem 방식)
    // -------------------------
    void ShowChoiceUI(int index, bool show)
    {
        if (index < 0 || index >= choiceUIItems.Count)
        {
            Debug.LogError("유효하지 않은 ChoiceUIItems 인덱스: " + index);
            return;
        }
        activeChoiceUIIndex = show ? index : -1;
        if (choiceUIItems[index].panel != null)
            choiceUIItems[index].panel.SetActive(show);
    }

    public void OnChoiceSelected(int choice)
    {
        userChoice = choice;
        if (activeChoiceUIIndex != -1)
        {
            choiceUIItems[activeChoiceUIIndex].panel.SetActive(false);
            activeChoiceUIIndex = -1;
        }
    }

    // -------------------------
    //      씬 전환 (리스트 방식)
    // -------------------------
    void ChangeScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            Debug.Log("씬 전환: " + sceneNames[sceneIndex]);
            SceneManager.LoadScene(sceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogError("유효하지 않은 씬 인덱스: " + sceneIndex);
        }
    }
}
