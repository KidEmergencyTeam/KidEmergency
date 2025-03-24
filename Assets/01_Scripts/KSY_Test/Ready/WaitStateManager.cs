using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaitStateManager : MonoBehaviour
{
    [Header("대기 상태 UI")]
    public TextMeshProUGUI waitStateText;

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
        UpdateUI();
    }

    // 플레이어 준비 완료 시 호출되는 콜백
    void OnPlayerReady()
    {
        UpdateUI();
        if (AllPlayersReady())
        {
            Debug.Log("모든 플레이어 준비 완료!");

            // 일정 시간 대기 후 씬 전환 실행
            Invoke("StartGame", gameStartDelay);
        }
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

    // UI 업데이트 (예: "대기중: 1/3")
    void UpdateUI()
    {
        int readyCount = 0;
        foreach (PlayerReady pr in playerReadyList)
        {
            if (pr.IsReady)
                readyCount++;
        }
        if (waitStateText != null)
            waitStateText.text = $"대기중: {readyCount}/{playerReadyList.Count}";
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
