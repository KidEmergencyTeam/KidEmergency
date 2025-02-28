using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class ChoiceUIItem
{
    public GameObject panel;       // 선택지 패널
    public List<Button> buttons;   // 해당 패널에 포함된 선택 버튼들 (1번, 2번, …)
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
    public TypingEffect typingEffect;         // 타이핑 효과 스크립트

    [Header("선택지")]
    public List<ChoiceUIItem> choiceUIItems;  // 선택지 패널+버튼 묶음 리스트

    // --------------------
    //   연기 파티클 관련
    // --------------------
    [Header("연기 파티클")]
    public ParticleSystem smokeEffect;

    // --------------------
    //      씬 전환
    // --------------------
    [Header("씬 이름")]
    public List<string> sceneNames; // [0] 복도, [1] 계단/엘리베이터, [2] 운동장 등

    // --------------------
    //  내부 상태값
    // --------------------
    private int userChoice = 0;      // 유저가 선택한 값 (1,2,…)
    private int currentStep = 1;     // 현재 진행 스텝 (1~37)
    private Dictionary<int, Func<IEnumerator>> scenarioSteps;
    private int activeChoiceUIIndex = -1; // 현재 활성화된 선택지 UI 인덱스

    // --------------------
    //  싱글톤 & DontDestroy
    // --------------------
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
        // 필수 컴포넌트 체크
        if (typingEffect == null)
        {
            Debug.LogError("TypingEffect가 할당되지 않았습니다.");
            return;
        }
        if (choiceUIItems == null || choiceUIItems.Count < 3)
        {
            Debug.LogError("ChoiceUIItems 리스트에 최소 3개 이상의 항목이 필요합니다.");
            return;
        }
        if (sceneNames == null || sceneNames.Count < 3)
        {
            Debug.LogError("Scene Names 리스트에 최소 3개 이상의 씬 이름이 필요합니다.");
            return;
        }

        // 모든 선택지 패널 비활성화
        foreach (var item in choiceUIItems)
        {
            if (item.panel != null) item.panel.SetActive(false);
        }

        // 각 선택지 패널의 버튼에 리스너 연결
        for (int i = 0; i < choiceUIItems.Count; i++)
        {
            foreach (Button btn in choiceUIItems[i].buttons)
            {
                int choiceNumber = choiceUIItems[i].buttons.IndexOf(btn) + 1;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnChoiceSelected(choiceNumber));
            }
        }

        // 시나리오 스텝 등록
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
    /// 전체 시나리오 실행 루프
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
    /// TypingEffect로 index번째 텍스트를 출력하고 대사가 끝날 때까지 대기
    /// </summary>
    IEnumerator PlayAndWait(int index)
    {
        typingEffect.PlayTypingAtIndex(index);
        yield return new WaitUntil(() => !typingEffect.IsTyping);
    }

    // -------------------------------------------------
    //               시나리오 스텝들
    // -------------------------------------------------

    // 1. (세티) 안녕 내 이름은 세티야…
    IEnumerator Step1()
    {
        yield return PlayAndWait(0);
    }

    // 2. (세티) 훈련이 시작되면 큰 소리가…
    IEnumerator Step2()
    {
        yield return PlayAndWait(1);
    }

    // 3. (시스템) 복도가 검은 연기로 서서히 차오른다 (연기 파티클 재생)
    IEnumerator Step3()
    {
        if (smokeEffect != null)
        {
            ParticleSystem ps = Instantiate(smokeEffect);
            ps.Play();
        }
        // 대사는 없으므로 바로 종료
        yield return null;
    }

    // 4. (세티) 복도를 봐! 화재가 발생한 거 같아…
    IEnumerator Step4()
    {
        yield return PlayAndWait(2);
    }

    // 5. (세티) 먼저 연기가 많이 나는 거 같으니…
    IEnumerator Step5()
    {
        yield return PlayAndWait(3);
    }

    // 6. (세티) 무엇으로 입과 코를 가릴래?
    IEnumerator Step6()
    {
        yield return PlayAndWait(4);
    }

    // 7. (시스템) 손과 물에 젖은 손수건 2개 중 택 1 선택지
    IEnumerator Step7()
    {
        // 첫 번째 선택지(0번 인덱스 패널) 표시
        ShowChoiceUI(0, true);
        yield return new WaitUntil(() => userChoice != 0);

        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(0, false);

        // 선택 결과: 1(손) → Step8 / 2(손수건) → Step11
        if (selected == 1)
        {
            currentStep = 8 - 1; // 다음 while에서 +1 되어 Step8
        }
        else
        {
            currentStep = 11 - 1; // Step11
        }
    }

    // 8. (시스템) 선택지 1 - 손을 골랐을 경우
    IEnumerator Step8()
    {
        // 대사 없이 “선택지 1” 안내만 있다고 가정 (필요하면 UI 연출 추가)
        yield return null;
    }

    // 9. (세티) 안돼! 연기가 많이 나는 화재 발생 시…
    IEnumerator Step9()
    {
        yield return PlayAndWait(5);
    }

    // 10. (세티) 주변에 손수건이나 천 등이 있다면…
    IEnumerator Step10()
    {
        yield return PlayAndWait(6);
        // 이후 13번 대사 씬으로 이동
        currentStep = 13 - 1;
    }

    // 11. (시스템) 선택지 2 - 손수건을 골랐을 경우
    IEnumerator Step11()
    {
        // 대사 없이 “선택지 2” 안내만 있다고 가정
        yield return null;
    }

    // 12. (세티) 잘했어!! 주변에 손수건이나 천 등이 있다면…
    IEnumerator Step12()
    {
        yield return PlayAndWait(7);
        // 이후 13번 대사 씬으로 이동
        currentStep = 13 - 1;
    }

    // 13. (세티) 모두 책상 위에 있는 손수건을 사용해서…
    IEnumerator Step13()
    {
        yield return PlayAndWait(8);
    }

    // 14. (시스템) 모든 유저가 손수건으로 입을 가리면…
    IEnumerator Step14()
    {
        // 대사 없이 오브젝트 활성화/이동 등 연출 처리
        yield return null;
    }

    // 15. (세티) 좋았어 복도로 나가면 먼저 맨 앞에 있는 사람이…
    IEnumerator Step15()
    {
        yield return PlayAndWait(9);

        // 교실 → 복도 씬 전환 (sceneNames[0])
        ChangeScene(0);
    }

    // 16. (세티) 자 복도로 나왔어! 연기가 좀 차 있으니까…
    IEnumerator Step16()
    {
        yield return PlayAndWait(10);
    }

    // 17. (세티) 먼저 제일 앞에 있는 사람이 화재 경보 벨을…
    IEnumerator Step17()
    {
        yield return PlayAndWait(11);
    }

    // 18. (시스템) 화재 경보 벨 누르는 연출
    IEnumerator Step18()
    {
        // 대사 없음. 버튼 누르면 경보음 등
        yield return null;
    }

    // 19. (세티) 잘했어! 화재가 일어나면…
    IEnumerator Step19()
    {
        yield return PlayAndWait(12);
    }

    // 20. (세티) 으아...연기가 점점 더 짙어지고 있어
    IEnumerator Step20()
    {
        yield return PlayAndWait(13);
    }

    // 21. (세티) 대피할 때는 코와 입을 막았어도…
    IEnumerator Step21()
    {
        yield return PlayAndWait(14);
    }

    // 22. (시스템) 유저가 몸을 숙이는 애니메이션
    IEnumerator Step22()
    {
        // 대사 없음. 애니메이션 or 시점 변경
        yield return null;
    }

    // 23. (세티) 자 몸을 숙이고 천천히 이동해보자…
    IEnumerator Step23()
    {
        yield return PlayAndWait(15);
    }

    // 24. (시스템) 선택지 1 - 피난 유도선 / 선택지 2 - 익숙한 길
    IEnumerator Step24()
    {
        // 두 번째 선택지(1번 인덱스 패널) 표시
        ShowChoiceUI(1, true);
        yield return new WaitUntil(() => userChoice != 0);

        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(1, false);

        if (selected == 1)
        {
            // 피난 유도선 선택
            currentStep = 25 - 1;
        }
        else
        {
            // 익숙한 길 선택
            currentStep = 26 - 1;
        }
    }

    // 25. (세티) 맞았어! 대단한데? 연기로 인해서 앞이 잘 안 보일때는…
    IEnumerator Step25()
    {
        yield return PlayAndWait(16);
        // 이후 Step28로 이동
        currentStep = 28 - 1;
    }

    // 26. (시스템) 선택지 2 - 익숙한 길
    IEnumerator Step26()
    {
        // 대사 없이 안내만
        yield return null;
    }

    // 27. (세티) 그러면 안 돼! 화재가 일어났을 때는…
    IEnumerator Step27()
    {
        yield return PlayAndWait(17);
        // 이후 Step28로 이동
        currentStep = 28 - 1;
    }

    // 28. (세티) 자, 그러면 침착하게 움직여보자!!
    IEnumerator Step28()
    {
        yield return PlayAndWait(18);

        // 복도 → 계단/엘리베이터 씬 (sceneNames[1])
        ChangeScene(1);
    }

    // 29. (세티) 휴 벌써 여기까지 왔어! 이제 내려가기만…
    IEnumerator Step29()
    {
        yield return PlayAndWait(19);
    }

    // 30. (세티) 계단과 엘리베이터가 있어, 어떻게 내려갈까??
    IEnumerator Step30()
    {
        yield return PlayAndWait(20);

        // 세 번째 선택지(2번 인덱스 패널) 표시
        ShowChoiceUI(2, true);
        yield return new WaitUntil(() => userChoice != 0);

        int selected = userChoice;
        userChoice = 0;
        ShowChoiceUI(2, false);

        // 1번(계단) → Step31 / 2번(엘리베이터) → Step33
        if (selected == 1)
        {
            currentStep = 31 - 1;
        }
        else
        {
            currentStep = 33 - 1;
        }
    }

    // 31. (시스템) 선택지1 - 계단
    IEnumerator Step31()
    {
        // 대사 없이 안내
        yield return null;
    }

    // 32. (세티) 맞았어! 화재 대피를 할 때는…
    IEnumerator Step32()
    {
        yield return PlayAndWait(21);
        // 이후 Step35로 이동
        currentStep = 35 - 1;
    }

    // 33. (시스템) 선택지2 - 엘리베이터
    IEnumerator Step33()
    {
        // 대사 없이 안내
        yield return null;
    }

    // 34. (세티) 안돼! 화재 시에는 엘리베이터를 타고 대피했다가…
    IEnumerator Step34()
    {
        yield return PlayAndWait(22);
        // 이후 Step35로 이동
        currentStep = 35 - 1;
    }

    // 35. (세티) 계단으로 침착하게 내려가자…
    IEnumerator Step35()
    {
        yield return PlayAndWait(23);

        // 계단/엘리베이터 → 운동장 씬 (sceneNames[2])
        ChangeScene(2);
    }

    // 36. (세티) 와! 안전하게 모두 빠져나왔어…
    IEnumerator Step36()
    {
        yield return PlayAndWait(24);
    }

    // 37. (세티) 같은 사건이 일어나도 오늘처럼만 하면…
    IEnumerator Step37()
    {
        yield return PlayAndWait(25);
        // 시나리오 종료 후 추가 연출 등
    }

    // -------------------------------------------------
    //             선택지 UI 표시/숨김
    // -------------------------------------------------
    void ShowChoiceUI(int index, bool show)
    {
        if (index < 0 || index >= choiceUIItems.Count)
        {
            Debug.LogError($"유효하지 않은 ChoiceUIItems 인덱스: {index}");
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

    // -------------------------------------------------
    //              씬 전환 메서드
    // -------------------------------------------------
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
