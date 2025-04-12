using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ChoiceType
{
    A,
    B
}

[Serializable]
public class SceneAnswer
{
    [Header("씬 이름")]
    public string sceneName;

    [Header("정답 버튼")]
    public ChoiceType correctChoice;
}

public class OptionChoice : DisableableSingleton<OptionChoice>
{
    [Header("씬별 정답 버튼 설정")]
    public List<SceneAnswer> sceneAnswers;

    [Header("투표 버튼")]
    public Button buttonA;
    public Button buttonB;

    [Header("선택지 처리 대기 시간")]
    public float choiceDelay = 0.1f;

    // 투표 결과 콜백 (true: 정답, false: 오답)
    private Action<bool> resultCallback;

    // 선택지 투표
    public IEnumerator StartVote(Action<bool> onResult)
    {
        resultCallback = onResult;

        // 버튼 null 체크
        if (buttonA == null || buttonB == null)
        {
            Debug.LogError("[OptionChoice] 버튼 -> null");
            yield break;
        }

        // 버튼 오브젝트 활성화
        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(true);

        // 현재 활성 씬 이름을 가져옴
        string currentScene = SceneManager.GetActiveScene().name;

        // sceneAnswers 리스트를 순회하며 현재 씬과 일치하는 정답 버튼을 찾음
        SceneAnswer foundAnswer = null;
        if (sceneAnswers != null)
        {
            for (int i = 0; i < sceneAnswers.Count; i++)
            {
                if (sceneAnswers[i].sceneName == currentScene)
                {
                    foundAnswer = sceneAnswers[i];
                    break;
                }
            }
        }

        if (foundAnswer == null)
        {
            Debug.LogError($"[OptionChoice] 현재 씬과 일치하는 정답 버튼이 없습니다.");
            yield break;
        }
        ChoiceType targetCorrectChoice = foundAnswer.correctChoice;

        // 중복 등록 방지를 위해 기존 버튼 이벤트 제거
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();

        bool voteCompleted = false;
        ChoiceType voteResult = ChoiceType.A; 

        // 버튼 클릭 시 선택 결과를 기록하고 UIManager의 옵션 UI를 닫음
        buttonA.onClick.AddListener(() =>
        {
            voteResult = ChoiceType.A;
            voteCompleted = true;
            UIManager.Instance.CloseAllOptionUI();
        });

        buttonB.onClick.AddListener(() =>
        {
            voteResult = ChoiceType.B;
            voteCompleted = true;
            UIManager.Instance.CloseAllOptionUI();
        });

        // 플레이어의 선택지 투표를 대기
        yield return new WaitUntil(() => voteCompleted);

        // 선택지 처리 이후 일정 시간 대기
        yield return new WaitForSeconds(choiceDelay);

        // 버튼 이벤트 제거
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();

        // 선택 결과와 씬에 설정된 정답을 비교하여 콜백 실행
        bool isCorrect = voteResult == targetCorrectChoice;
        resultCallback?.Invoke(isCorrect);
    }
}
