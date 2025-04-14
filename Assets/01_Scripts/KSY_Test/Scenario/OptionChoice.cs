using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 버튼 선택
public enum ChoiceType
{
    A,
    B
}

[Serializable]
public class SceneAnswer
{
    [Header("텍스트 및 이미지 등 새로 적용할 씬 이름")]
    public string sceneName;

    // 선택지 A
    [Header("정답 버튼")]
    public ChoiceType correctChoice;

    [Header("버튼 A 새 옵션 텍스트")]
    public string newTextA;

    [Header("버튼 A 새 옵션 강조할 문자")]
    public string newHighlightTextA;

    [Header("버튼 A 새 옵션 이미지")]
    public Sprite newSpriteA;

    // 선택지 B
    [Header("버튼 B 새 옵션 텍스트")]
    public string newTextB;

    [Header("버튼 B 새 옵션 강조할 문자")]
    public string newHighlightTextB;

    [Header("버튼 B 새 옵션 이미지")]
    public Sprite newSpriteB;
}

public class OptionChoice : DisableableSingleton<OptionChoice>
{
    [Header("씬별 정답 버튼 및 옵션 설정")]
    public List<SceneAnswer> sceneAnswers;

    [Header("선택 버튼")]
    public Button buttonA;
    public Button buttonB;

    [Header("버튼별 OptionUI")]
    public OptionUI optionUI_A;
    public OptionUI optionUI_B;

    [Header("선택지 처리 대기 시간")]
    public float choiceDelay = 0.1f;

    // 투표 결과 -> true: 정답, false: 오답
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

        // 현재 실행 중인 씬 이름을 가져옴
        string currentScene = SceneManager.GetActiveScene().name;

        // sceneAnswers 리스트를 순회하며 현재 씬과 일치하는 옵션 정보를 찾음 -> 텍스트 및 이미지
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
        else
        {
            Debug.LogError("[OptionChoice] 현재 씬과 일치하는 옵션 정보가 없습니다.");
            yield break;
        }

        // 현재 씬과 일치하는 정답 버튼 저장
        ChoiceType targetCorrectChoice = foundAnswer.correctChoice;

        // A 선택지 -> 새로운 텍스트 및 이미지 반영
        if (optionUI_A != null)
        {
            if (optionUI_A.optionText != null)
            {
                optionUI_A.optionText.text = foundAnswer.newTextA;
                optionUI_A.highlightText = foundAnswer.newHighlightTextA;
                optionUI_A.optionImage.sprite = foundAnswer.newSpriteA;
            }
        }
        else
        {
            if (optionUI_A == null)
            {
                Debug.LogError("[OptionChoice] OptionUI A -> null");
                yield break;
            }
        }

        // B 선택지 -> 새로운 텍스트 및 이미지 반영
        if (optionUI_B != null)
        {
            if (optionUI_B.optionText != null)
            {
                optionUI_B.optionText.text = foundAnswer.newTextB;
                optionUI_B.highlightText = foundAnswer.newHighlightTextB;
                optionUI_B.optionImage.sprite = foundAnswer.newSpriteB;
            }

        }
        else
        {
            if (optionUI_B == null)
            {
                Debug.LogError("[OptionChoice] OptionUI B -> null");
                yield break;
            }
        }

        // 중복 등록 방지를 위해 기존 버튼 이벤트 제거
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();

        bool voteCompleted = false;
        ChoiceType voteResult = ChoiceType.A; 

        // 버튼 클릭 시 선택 결과를 기록하고 UIManager를 통해 전체 OptionUI 닫음
        buttonA.onClick.AddListener(() =>
        {
            voteResult = ChoiceType.A;
            voteCompleted = true;
            Debug.Log("버튼 A 클릭됨");
            
            // 전체 선택지 닫음
            buttonA.gameObject.SetActive(false);
            buttonB.gameObject.SetActive(false);
        });

        buttonB.onClick.AddListener(() =>
        {
            voteResult = ChoiceType.B;
            voteCompleted = true;
            Debug.Log("버튼 B 클릭됨");

            // 전체 선택지 닫음
            buttonA.gameObject.SetActive(false);
            buttonB.gameObject.SetActive(false);
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
