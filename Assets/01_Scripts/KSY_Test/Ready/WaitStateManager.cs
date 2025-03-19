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
    public string nextSceneName;

    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    [Header("페이드 효과")]
    public FadeInOut fadeInOut;

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
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("다음 씬 이름이 설정되어 있지 않습니다.");
            return;
        }
        Debug.Log("게임 시작!");
        StartCoroutine(LoadSceneWithFadeOutAsync());
    }

    // 페이드 아웃 효과 실행 후 비동기 씬 전환하는 코루틴
    IEnumerator LoadSceneWithFadeOutAsync()
    {
        // FadeInOut 컴포넌트가 연결되어 있지 않으면 바로 씬 전환
        if (fadeInOut == null)
        {
            Debug.LogError("FadeInOut 컴포넌트가 연결되어 있지 않습니다.");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield break;
        }

        // 페이드 아웃 효과 실행
        yield return StartCoroutine(fadeInOut.FadeOut());

        // 비동기로 씬 전환
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
