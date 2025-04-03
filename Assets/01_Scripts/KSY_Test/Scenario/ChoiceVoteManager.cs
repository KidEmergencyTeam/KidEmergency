using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceVoteManager : DisableableSingleton<ChoiceVoteManager>
{
    // 인스펙터에서 UI 패널 프리팹과 관련 설정을 할당
    [Serializable]
    public class ChoiceUIPanelSceneReference
    {
        // 모든 플레이어가 사용할 공용 UI 패널 프리팹 (프리팹은 에셋이므로 직접 수정할 수 없으며, 인스턴스화해서 사용)
        public GameObject panelPrefab;

        // 동률 발생 시 우선할 선택.
        // 예를 들어 1이면 첫 번째 선택지, 2면 두 번째 선택지를 우선 처리
        [Header("동률 발생 시 우선할 선택")]
        public int tieChoiceIndex;
    }

    [Header("선택지 패널 프리팹 설정")]
    public List<ChoiceUIPanelSceneReference> choicePanelSceneReferences;

    [Header("선택지 처리 대기 시간")]
    public float choiceDelay = 2f;

    // 선택지 라벨 배열. 예: 첫 번째 선택지는 "A", 두 번째 선택지는 "B"
    private string[] choiceLabels = { "A", "B" };

    // 공용 UI 패널과 해당 구성요소(Button, Text 등)를 저장하는 클래스
    public class InstantiatedChoicePanel
    {
        // 인스턴스화된 공용 UI 패널 오브젝트 (씬 내 오브젝트)
        public GameObject panel;
        // 패널 내에 포함된 Button
        public List<Button> buttons;
        // 패널 내에 포함된 TextMeshProUGUI
        public List<TextMeshProUGUI> voteCountTexts;
        // 동률 처리 시 우선할 선택 값 (예: 1이면 첫 번째 선택지)
        public int tieChoiceIndex;
    }

    // 공용 패널을 저장 (각 플레이어별로 개별 패널이 아닌 하나의 패널 사용)
    private InstantiatedChoicePanel commonPanel = null;

    // 각 플레이어의 투표 결과 저장 
    // 플레이어의 중복 투표를 막기 위해, 플레이어별로 한 번씩만 투표하도록 처리
    private Dictionary<GameObject, int> playerVotes = new Dictionary<GameObject, int>();

    // 전체 플레이어 수
    private int totalPlayers = 0;

    // 최종 선택 결과
    private int finalVoteResult = 0;

    // 투표 완료 후 최종 결과를 전달할 콜백 함수
    private Action<int> resultCallback;

    // 모든 플레이어에게 공통된 선택지 UI 패널을 사용하여 투표를 받고,
    // 투표가 완료되면 집계하여 최종 결과를 반환하는 코루틴
    public IEnumerator ShowChoiceAndGetResult(int choicePanelSceneReferenceIndex, Action<int> onResult)
    {
        // 투표 시작 전 변수 초기화
        finalVoteResult = 0;
        playerVotes.Clear();
        resultCallback = onResult;

        // 태그가 "Player"인 모든 오브젝트를 찾음
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        totalPlayers = players.Length;
        if (totalPlayers == 0)
        {
            Debug.LogWarning("[ChoiceVoteManager] 플레이어가 없습니다. 기본 결과 1 반환");
            onResult?.Invoke(1);
            yield break;
        }

        // 씬 오브젝트 목록 인덱스가 올바른지 검사
        if (choicePanelSceneReferenceIndex < 0 || choicePanelSceneReferenceIndex >= choicePanelSceneReferences.Count)
        {
            Debug.LogError("[ChoiceVoteManager] 잘못된 씬 오브젝트 목록 인덱스: " + choicePanelSceneReferenceIndex);
            onResult?.Invoke(1);
            yield break;
        }

        // 선택한 프리팹 설정 정보와 동률 처리 우선순위 값 가져오기
        ChoiceUIPanelSceneReference sceneReference = choicePanelSceneReferences[choicePanelSceneReferenceIndex];
        int tieChoiceIndex = sceneReference.tieChoiceIndex;

        // 씬 내에서 UI 패널이 배치될 캔버스 태그로 찾기
        GameObject canvas = GameObject.FindGameObjectWithTag("PanelCanvas");
        if (canvas == null)
        {
            Debug.LogError("[ChoiceVoteManager] PanelCanvas 태그가 적용된 캔버스를 찾을 수 없습니다. 태그 설정을 확인하세요.");
            onResult?.Invoke(1);
            yield break;
        }

        // 공용 UI 패널 프리팹이 할당되어 있는지 확인
        if (sceneReference.panelPrefab == null)
        {
            Debug.LogError("[ChoiceVoteManager] 할당된 공용 UI 패널 프리팹이 없습니다.");
            onResult?.Invoke(1);
            yield break;
        }

        // 공용 UI 패널 인스턴스화
        GameObject panelInstance = Instantiate(sceneReference.panelPrefab, canvas.transform);
        panelInstance.SetActive(true);

        commonPanel = new InstantiatedChoicePanel();
        commonPanel.panel = panelInstance;
        commonPanel.buttons = new List<Button>(panelInstance.GetComponentsInChildren<Button>());
        commonPanel.voteCountTexts = new List<TextMeshProUGUI>(panelInstance.GetComponentsInChildren<TextMeshProUGUI>());
        commonPanel.tieChoiceIndex = tieChoiceIndex;

        // 투표 시작 전에 초기 텍스트 설정 ("선택지 A: 0/총 참여자수")
        ResetVoteTexts(commonPanel, totalPlayers);

        // 공용 패널의 버튼 이벤트 등록 (단 한 번만 등록)
        // 버튼 클릭 시 호출하는 Vote()는, 예제에서는 현재 로컬 플레이어의 투표로 처리
        // 실제 멀티플레이어 환경이라면 각 플레이어가 개별 입력을 통해 Vote()를 호출해야 함
        for (int i = 0; i < commonPanel.buttons.Count; i++)
        {
            int choiceVal = i + 1;
            Button btn = commonPanel.buttons[i];
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                // 여기서는 예시로 현재 로컬 플레이어(예: 첫 번째 플레이어 객체)를 사용
                // 실제 구현에서는 각 플레이어의 입력에 따라 해당 플레이어 객체를 전달해야 함
                Vote(players[0], choiceVal);
            });
        }

        // 모든 플레이어가 투표할 때까지 대기
        yield return new WaitUntil(() => playerVotes.Count >= totalPlayers);

        // 결과를 표시하기 위해 설정한 지연 시간만큼 대기
        yield return new WaitForSeconds(choiceDelay);

        // 모든 투표를 집계하여 최종 선택 결정 (동률 시 tieChoiceIndex 사용)
        finalVoteResult = CalculateMajorityWithTie(tieChoiceIndex);

        // 최종 결과 반영: 공용 패널 업데이트 후 파괴 처리
        UpdateVoteUI(commonPanel);
        Destroy(commonPanel.panel);
        commonPanel = null;

        // 선택지 처리 후 InputAction을 재활성화
        RayController2 rayController = FindObjectOfType<RayController2>();
        if (rayController != null)
        {
            rayController.ReactivateInputActions();
        }

        // 최종 결과 콜백 호출 (다른 로직으로 최종 결과 전달)
        resultCallback?.Invoke(finalVoteResult);
    }

    // 특정 플레이어의 투표를 기록 (중복 투표는 무시)
    // 실제 멀티플레이 환경에서는 각 플레이어의 입력에 의해 이 함수가 호출되어야 함
    public void Vote(GameObject player, int choice)
    {
        // player가 null이면 (예: 로컬 입력 처리) 기록하지 않고 바로 반환할 수 있음
        if (player == null) return;

        // 이미 투표한 경우 무시 (한 플레이어당 한 번 투표)
        if (playerVotes.ContainsKey(player)) return;

        // 플레이어의 투표 기록 추가
        playerVotes.Add(player, choice);

        // 단일 플레이어인 경우 즉시 결과 처리
        if (totalPlayers == 1)
        {
            finalVoteResult = choice;
            UpdateVoteUI(commonPanel);
            return;
        }

        // 공용 패널의 투표 현황 업데이트
        UpdateVoteUI(commonPanel);
    }

    // 모든 플레이어의 투표 결과를 집계하여 최다 득표 선택을 결정
    // 동률인 경우, 전달받은 tieChoiceIndex (예: 1이면 첫 번째 선택지)를 사용
    private int CalculateMajorityWithTie(int tieChoiceIndex)
    {
        // 각 선택지별 투표 수를 집계할 딕셔너리 생성
        Dictionary<int, int> voteCounts = new Dictionary<int, int>();
        foreach (var vote in playerVotes.Values)
        {
            if (!voteCounts.ContainsKey(vote))
                voteCounts[vote] = 0;
            voteCounts[vote]++;
        }

        int maxCount = 0;
        List<int> topCandidates = new List<int>();

        // 최다 득표 수와 해당 선택지를 찾음
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

        // 단일 후보가 있을 경우 해당 선택지를 반환
        if (topCandidates.Count == 1)
        {
            return topCandidates[0];
        }
        else
        {
            // 동률인 경우, tieChoiceIndex 사용
            int tiePick = tieChoiceIndex;
            if (tiePick < 1) tiePick = 1;
            // 버튼 개수를 기준으로 범위를 조정 (버튼 순서대로 선택지가 매핑)
            if (commonPanel != null && tiePick > commonPanel.buttons.Count)
                tiePick = commonPanel.buttons.Count;
            Debug.Log($"[ChoiceVoteManager] 동률 발생 -> tieChoiceIndex({tiePick}) 사용");
            return tiePick;
        }
    }

    // 투표 현황 텍스트 업데이트
    // 예: "선택지 A: 2/3"처럼 표시 (2표가 들어왔으며, 총 3명의 참여자)
    private void UpdateVoteUI(InstantiatedChoicePanel panel)
    {
        if (panel == null) return;

        // 각 선택지(버튼)별 투표 수 집계
        int[] counts = new int[panel.buttons.Count];
        foreach (var vote in playerVotes.Values)
        {
            int idx = vote - 1;
            if (idx >= 0 && idx < counts.Length)
                counts[idx]++;
        }

        // 각 선택지별 집계 결과 텍스트 업데이트
        for (int i = 0; i < counts.Length; i++)
        {
            if (i < panel.voteCountTexts.Count && panel.voteCountTexts[i] != null)
            {
                string label = (i < choiceLabels.Length) ? $"선택지 {choiceLabels[i]}" : $"선택지 {i + 1}";
                panel.voteCountTexts[i].text = $"{label}: {counts[i]}/{totalPlayers}";
            }
        }
    }

    // 투표 시작 전에 UI 패널 내의 텍스트를 초기화
    // 각 선택지는 "선택지 라벨: 0/총 참여자수"로 설정
    private void ResetVoteTexts(InstantiatedChoicePanel panel, int total)
    {
        if (panel == null) return;
        for (int i = 0; i < panel.voteCountTexts.Count; i++)
        {
            if (panel.voteCountTexts[i] != null)
            {
                string label = (i < choiceLabels.Length) ? $"선택지 {choiceLabels[i]}" : $"선택지 {i + 1}";
                panel.voteCountTexts[i].text = $"{label}: 0/{total}";
            }
        }
    }
}
