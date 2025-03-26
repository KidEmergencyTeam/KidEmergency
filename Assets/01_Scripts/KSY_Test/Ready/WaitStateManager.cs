using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 준비 처리
public class WaitStateManager : MonoBehaviour
{
    [Header("대기 상태 UI")]
    public Image readyIndicatorImage;

    [Header("다음 씬 이름")]
    public string SceneName;

    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    // 대기 중인 플레이어들의 PlayerReady.cs를 저장할 리스트
    private List<PlayerReady> playerReadyList = new List<PlayerReady>();

    void Start()
    {
        // 태그가 "Player"인 모든 오브젝트에서 PlayerReady.cs 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerReady pr = player.GetComponent<PlayerReady>();
            if (pr != null)
            {
                playerReadyList.Add(pr);
                pr.onPlayerReady += OnPlayerReady;
            }
        }
    }

    // 전체 준비 완료 시 이미지 처리 및 일정 시간 대기 후 씬 전환 실행
    // 추후에 멀티 구현 시 개별적으로 이미지를 처리하는 기능을 구현해야 함 
    void OnPlayerReady()
    {
        if (AllPlayersReady())
        {
            Debug.Log("모든 플레이어 준비 완료!");

            // 준비 완료 이미지 활성화 
            if (readyIndicatorImage != null)
            {
                readyIndicatorImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("readyIndicatorImage가 할당되어 있지 않습니다.");
            }

            // 코루틴을 통해 일정 시간 대기 후 씬 전환 실행
            StartCoroutine(WaitAndStartGame());
        }
    }

    // 일정 시간 대기 후 StartGame()을 실행하는 코루틴
    private IEnumerator WaitAndStartGame()
    {
        yield return new WaitForSeconds(gameStartDelay);
        StartGame();
    }

    // 모든 플레이어가 준비되었는지 확인
    bool AllPlayersReady()
    {
        foreach (PlayerReady pr in playerReadyList)
        {
            if (!pr.IsReady)
                return false;
        }
        return true;
    }

    // 모든 플레이어 준비 완료 시 씬 전환 실행
    void StartGame()
    {
        if (string.IsNullOrEmpty(SceneName))
        {
            Debug.LogError("다음 씬 이름이 설정되어 있지 않습니다.");
            return;
        }
        Debug.Log("게임 시작!");
        StartCoroutine(LoadSceneWithFadeOutAsync());
    }

    // 비동기 방식으로 씬 전환
    IEnumerator LoadSceneWithFadeOutAsync()
    {
        // FadeInOut 싱글톤 인스턴스가 null 상태라면
        // 페이드 인/아웃 효과 없이 바로 씬 전환
        if (FadeInOut.Instance == null)
        {
            Debug.LogError("FadeInOut 싱글톤 인스턴스가 연결되어 있지 않습니다.");

            // 씬 전환 중에도 게임이 멈추지 않고 계속 실행
            // 추후에 로딩 중 "로딩중"이라는 문구나 로딩바 같은 UI 요소를 표시 가능
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!asyncLoad.isDone)
            {
                // 로딩 대기
                yield return null;
            }

            // 인스턴스가 null인 상태라면 코루틴 종료
            yield break;
        }

        // FadeInOut 싱글톤 인스턴스가 정상적으로 존재하는 경우
        // 1. 페이드 아웃 효과 실행
        yield return StartCoroutine(FadeInOut.Instance.FadeOut());

        // 2. 씬 전환 
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            // 로딩 대기
            yield return null;
        }

        // 3. 씬 로드 후 페이드 인 효과 실행
        // ScenarioManager.cs -> Start에서 코루틴으로 처리 (페이드 인 효과 이후 시나리오 진행)
    }
}
