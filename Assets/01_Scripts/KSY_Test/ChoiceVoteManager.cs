using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceVoteManager : MonoBehaviour
{
    public static ChoiceVoteManager Instance { get; private set; }

    [Serializable]
    public class ChoiceUIPanel
    {
        public GameObject panel;
        public List<Button> buttons;
        public List<TextMeshProUGUI> voteCountTexts;

        // 동률 발생 시
        // 1을 기재하면 첫 번째 선택지를, 
        // 2를 기재하면 두 번째 선택지를 우선하여 처리
        [Header("동률 발생 시 우선할 선택")]
        public int tieChoiceIndex;
    }

    [Header("선택지 패널 목록")]
    public List<ChoiceUIPanel> choicePanels;

    [Header("선택지 처리 대기 시간")]
    public float choiceDelay = 1f;

    // 선택지 라벨 -> 선택지 A, 선택지 B
    private string[] choiceLabels = { "A", "B"};

    private Dictionary<GameObject, int> playerVotes;
    private int totalPlayers = 0;
    private bool votingFinished = false;
    private int finalVoteResult = 0;

    // 현재 열려있는 패널 (동률 해석 시 tieChoiceIndex 참고)
    private ChoiceUIPanel currentPanel = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 시작 시 패널 비활성화
        foreach (var panel in choicePanels)
        {
            if (panel.panel != null)
                panel.panel.SetActive(false);
            ResetVoteTexts(panel, 0);
        }
    }

    // choicePanelIndex에 해당하는 UI를 열고, 태그 플레이어인 인원 모두 투표 후
    // 동률이면 currentPanel.tieChoiceIndex를 우선하여 결과 결정
    public IEnumerator ShowChoiceAndGetResult(int choicePanelIndex, Action<int> onResult)
    {
        votingFinished = false;
        finalVoteResult = 0;
        playerVotes = new Dictionary<GameObject, int>();
        currentPanel = null;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        totalPlayers = players.Length;
        if (totalPlayers == 0)
        {
            Debug.LogWarning("[ChoiceVoteManager] Player가 없습니다. 결과=1");
            onResult?.Invoke(1);
            yield break;
        }

        if (choicePanelIndex < 0 || choicePanelIndex >= choicePanels.Count)
        {
            Debug.LogError("[ChoiceVoteManager] 잘못된 choicePanelIndex: " + choicePanelIndex);
            onResult?.Invoke(1);
            yield break;
        }

        currentPanel = choicePanels[choicePanelIndex];
        if (currentPanel.panel != null)
            currentPanel.panel.SetActive(true);

        // 투표 텍스트 초기화
        ResetVoteTexts(currentPanel, totalPlayers);

        // 버튼 등록
        for (int i = 0; i < currentPanel.buttons.Count; i++)
        {
            int choiceVal = i + 1;
            Button btn = currentPanel.buttons[i];
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                var whoClicked = FindAnyPlayer();
                Vote(whoClicked, choiceVal);
            });
        }

        // 전원 투표까지 대기
        yield return new WaitUntil(() => votingFinished);

        // 최종 득표수 업데이트 & 1초 대기
        UpdateVoteUI(currentPanel);
        yield return new WaitForSeconds(choiceDelay);

        if (currentPanel.panel != null)
            currentPanel.panel.SetActive(false);

        onResult?.Invoke(finalVoteResult);
    }

    public void Vote(GameObject player, int choice)
    {
        if (player == null) return;
        // 중복투표 방지
        if (playerVotes.ContainsKey(player)) return; 

        playerVotes.Add(player, choice);

        // 1명인 경우 즉시 확정
        if (totalPlayers == 1)
        {
            finalVoteResult = choice;
            votingFinished = true;
            UpdateVoteUI(currentPanel);
            return;
        }

        // 실시간 득표 UI 갱신
        UpdateVoteUI(currentPanel);

        // 전원 투표 => 최종 결정
        if (playerVotes.Count >= totalPlayers)
        {
            finalVoteResult = CalculateMajorityWithTie(currentPanel);
            votingFinished = true;
        }
    }

    // 득표수를 계산하여 최다 득표를 찾고, 동률이면 currentPanel.tieChoiceIndex 우선
    private int CalculateMajorityWithTie(ChoiceUIPanel panel)
    {
        // 먼저 choice별 득표수 집계
        Dictionary<int, int> voteCounts = new Dictionary<int, int>();
        foreach (var kv in playerVotes)
        {
            int c = kv.Value;
            if (!voteCounts.ContainsKey(c))
                voteCounts[c] = 0;
            voteCounts[c]++;
        }

        // 최다 득표수와 그 후보들 찾기
        int maxCount = 0;
        List<int> topCandidates = new List<int>();
        foreach (var pair in voteCounts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                topCandidates.Clear();
                topCandidates.Add(pair.Key);
            }
            else if (pair.Value == maxCount)
            {
                topCandidates.Add(pair.Key);
            }
        }

        // 동률이 아닐 때 (득표수가 가장 높은 후보가 하나인 경우)
        if (topCandidates.Count == 1)
        {
            // 단일 후보를 반환
            return topCandidates[0];
        }
        else
        {
            // 동률인 경우
            // UI 패널에 설정된 tieChoiceIndex 값을 사용하여 결정함 -> 1이면 1번 선택지, 2면 2번 선택지 결정
            // tiePick: 동률 상황에서 최종 선택지를 결정하기 위해 사용
            int tiePick = panel.tieChoiceIndex;

            // 버튼 갯 수 만큼 선택지 범위 조정
            if (tiePick < 1) tiePick = 1;
            if (tiePick > panel.buttons.Count) tiePick = panel.buttons.Count;

            Debug.Log($"[ChoiceVoteManager] 동률 발생 -> tieChoiceIndex({tiePick}) 사용");

            // 조정된 tiePick 값을 반환
            return tiePick;
        }

    }

    // 득표 현황을 "선택지 라벨: x/y"로 표시
    private void UpdateVoteUI(ChoiceUIPanel panel)
        {
            if (panel == null) return;

            int[] counts = new int[panel.buttons.Count];
            foreach (var kv in playerVotes)
            {
                int c = kv.Value;
                int idx = c - 1;
                if (idx >= 0 && idx < counts.Length)
                    counts[idx]++;
            }

            for (int i = 0; i < counts.Length; i++)
            {
                if (i < panel.voteCountTexts.Count && panel.voteCountTexts[i] != null)
                {
                    // 선택지 라벨로 표시
                    string label = (i < choiceLabels.Length) ? $"선택지 {choiceLabels[i]}"
                                                             : $"선택지 {i + 1}";
                    panel.voteCountTexts[i].text = $"{label}: {counts[i]}/{totalPlayers}";
                }
            }
        }

        // 투표가 끝난 후에 투표 결과를 초기화
        private void ResetVoteTexts(ChoiceUIPanel panel, int total)
        {
            if (panel == null) return;
            for (int i = 0; i < panel.voteCountTexts.Count; i++)
            {
                if (panel.voteCountTexts[i] != null)
                {
                    // 동일하게 선택지 라벨: 0/total
                    string label = (i < choiceLabels.Length) ? $"선택지 {choiceLabels[i]}"
                                                             : $"선택지 {i + 1}";
                    panel.voteCountTexts[i].text = $"{label}: 0/{total}";
                }
            }
        }

    // 태그가 플레이어인 모든 오브젝트들을 찾아 배열로 저장
    private GameObject FindAnyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0) return players[0];
        return null;
    }
}