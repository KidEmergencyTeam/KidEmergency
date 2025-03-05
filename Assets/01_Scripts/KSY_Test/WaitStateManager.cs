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

    // 리스트에 포함된 PlayerReady.cs 수만큼 대기 인원 표시
    private List<PlayerReady> playerReadyList = new List<PlayerReady>();

    void Start()
    {
        // 태그가 플레이어인 모든 오브젝트를 찾아
        // PlayerReady.cs를 리스트에 추가
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerReady pr = player.GetComponent<PlayerReady>();
            if (pr != null)
            {
                playerReadyList.Add(pr);

                // 각 플레이어가 준비되었을 때 이벤트를 통해 콜백 받음
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

            // 일정 시간 대기 후 다음씬으로 전환
            Invoke("StartGame", gameStartDelay);
        }
    }

    // 모든 플레이어가 준비 완료 상태인지 확인
    bool AllPlayersReady()
    {
        foreach (PlayerReady pr in playerReadyList)
        {
            // 플레이어 준비 상태 체크
            if (!pr.IsReady)
                return false;
        }
        return true;
    }

    // UI 업데이트 처리 (예: "대기중: 1/3" 형태)
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

    // 모든 플레이어 준비 완료 시 씬 전환
    void StartGame()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("다음 씬 이름이 설정되어 있지 않습니다.");
            return;
        }

        Debug.Log("게임 시작!");
        SceneManager.LoadScene(nextSceneName);
    }
}
