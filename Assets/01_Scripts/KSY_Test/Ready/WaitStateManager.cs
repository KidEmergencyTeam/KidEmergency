using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// 모든 플레이어의 준비 완료 상태를 받아 
// 다음씬으로 전환해 주는 스크립트
public class WaitStateManager : MonoBehaviour
{
    [Header("대기 상태 UI")]
    public TextMeshProUGUI waitStateText;

    [Header("다음 씬 이름")]
    public string nextSceneName;

    [Header("게임 시작 대기 시간")]
    public float gameStartDelay = 2f;

    // 페이드 효과 오브젝트(씬에 존재하는 FadeInOut 컴포넌트)
    [Header("페이드 효과")]
    public FadeInOut fadeInOut;

    // 대기 중인 플레이어들의 PlayerReady.cs를 저장할 리스트
    // 목적: 리스트에 포함된 PlayerReady.cs 수만큼 대기 인원 처리 -> 인원 표시 및 준비 상태 받아서 씬 전환
    private List<PlayerReady> playerReadyList = new List<PlayerReady>();

    void Start()
    {
        // 태그가 "Player"인 모든 오브젝트에서 PlayerReady.cs 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            // PlayerReady.cs 가져오기
            PlayerReady pr = player.GetComponent<PlayerReady>();

            // PlayerReady.cs 존재하면,
            if (pr != null)
            {
                // 리스트에 추가
                playerReadyList.Add(pr);

                // 플레이어가 준비되었을 때 콜백 등록
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
        StartCoroutine(LoadSceneWithFadeOut());
    }

    // 페이드 아웃 효과 실행 후 씬 전환하는 코루틴
    IEnumerator LoadSceneWithFadeOut()
    {
        // fadeInOut 컴포넌트가 연결되어 있지 않으면 에러 로그 후 씬 전환
        if (fadeInOut == null)
        {
            Debug.LogError("FadeInOut 컴포넌트가 연결되어 있지 않습니다.");
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        // FadeInOut의 페이드 아웃 효과 실행 후 완료될 때까지 대기
        yield return StartCoroutine(fadeInOut.FadeOut());

        // 페이드 아웃 완료 후 씬 전환
        SceneManager.LoadScene(nextSceneName);
    }
}
