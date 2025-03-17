using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceVoteManager : MonoBehaviour
{
    // 싱글톤 인스턴스. 다른 스크립트에서는 ChoiceVoteManager.Instance로 접근 가능
    public static ChoiceVoteManager Instance { get; private set; }

    // 인스펙터에서 UI 패널 씬 오브젝트와 관련 설정을 할당
    [Serializable]
    public class ChoiceUIPanelSceneReference
    {
        // 각 플레이어가 사용할 UI 패널 (씬 내에 배치된 오브젝트, 기본적으로 비활성화 상태여야 함)
        public List<GameObject> panelInstances;

        // 동률 발생 시 우선할 선택.
        // 예를 들어 1이면 첫 번째 선택지, 2면 두 번째 선택지를 우선 처리
        [Header("동률 발생 시 우선할 선택")]
        public int tieChoiceIndex;
    }

    [Header("선택지 패널 씬 오브젝트 목록 (프리팹 사용하지 않음)")]
    public List<ChoiceUIPanelSceneReference> choicePanelSceneReferences;

    [Header("선택지 처리 대기 시간")]
    public float choiceDelay = 2f;

    // 선택지 라벨 배열. 예를 들어 첫 번째 선택지는 "A", 두 번째 선택지는 "B"로 사용됨.
    private string[] choiceLabels = { "A", "B" };

    // 활성화된 UI 패널과 해당 구성요소(Button, Text 등)를 저장하는 클래스
    public class InstantiatedChoicePanel
    {
        // 활성화된 UI 패널 오브젝트 (씬 내 오브젝트)
        public GameObject panel;
        // 패널 내에 포함된 Button
        public List<Button> buttons;
        // 패널 내에 포함된 TextMeshProUGUI
        public List<TextMeshProUGUI> voteCountTexts;
        // 동률 처리 시 우선할 선택 값 (예: 1이면 첫 번째 선택지)
        public int tieChoiceIndex;
    }

    // 각 플레이어별로 할당된 UI 패널 오브젝트를 저장
    private Dictionary<GameObject, InstantiatedChoicePanel> playerPanels = new Dictionary<GameObject, InstantiatedChoicePanel>();

    // 각 플레이어의 투표 결과 저장 
    private Dictionary<GameObject, int> playerVotes = new Dictionary<GameObject, int>();

    // 전체 플레이어 수
    private int totalPlayers = 0;

    // 최종 선택 결과
    private int finalVoteResult = 0;

    // 투표 완료 후 최종 결과를 전달할 콜백 함수
    private Action<int> resultCallback;

    private void Awake()
    {
        // 싱글톤 패턴 적용: 인스턴스가 없으면 현재 인스턴스를 할당, 이미 있으면 파괴
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 모든 플레이어에게 선택지 UI 패널 씬 오브젝트를 활성화하여 투표를 받고,
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

        // 선택한 씬 오브젝트 목록 정보와 동률 처리 우선순위 값 가져오기
        ChoiceUIPanelSceneReference sceneReference = choicePanelSceneReferences[choicePanelSceneReferenceIndex];
        int tieChoiceIndex = sceneReference.tieChoiceIndex;

        // 씬 내에서 UI 패널이 배치된 캔버스 오브젝트 찾기 (패널이 캔버스의 자식이 아닐 경우 대비)
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("[ChoiceVoteManager] 캔버스를 찾을 수 없습니다. Canvas 이름을 확인하세요.");
            onResult?.Invoke(1);
            yield break;
        }

        // 할당된 UI 패널 씬 오브젝트의 개수가 플레이어 수보다 충분한지 확인
        if (sceneReference.panelInstances == null || sceneReference.panelInstances.Count < totalPlayers)
        {
            Debug.LogError("[ChoiceVoteManager] 할당된 UI 패널의 수가 충분하지 않습니다. 필요한 패널 수: " + totalPlayers);
            onResult?.Invoke(1);
            yield break;
        }

        // 각 플레이어마다 씬 내 UI 패널 오브젝트를 활성화 및 초기화, 버튼 이벤트 등록
        int panelIndex = 0;
        foreach (var player in players)
        {
            GameObject panelInstance = sceneReference.panelInstances[panelIndex];
            panelIndex++;

            // 패널이 캔버스의 자식이 아닌 경우, 자식으로 설정 (필요시)
            if (panelInstance.transform.parent != canvas.transform)
            {
                panelInstance.transform.SetParent(canvas.transform, false);
            }
            panelInstance.SetActive(true);

            // 새 UI 패널 객체 생성 및 초기화
            InstantiatedChoicePanel icp = new InstantiatedChoicePanel();
            icp.panel = panelInstance;

            // 패널 내의 Button 및 TextMeshProUGUI 컴포넌트 가져오기
            icp.buttons = new List<Button>(panelInstance.GetComponentsInChildren<Button>());
            icp.voteCountTexts = new List<TextMeshProUGUI>(panelInstance.GetComponentsInChildren<TextMeshProUGUI>());

            // 동률 처리 우선 순위 설정
            icp.tieChoiceIndex = tieChoiceIndex;

            // 투표 시작 전에 초기 텍스트 설정 ("선택지 A: 0/총 참여자수")
            ResetVoteTexts(icp, totalPlayers);

            // 각 버튼에 대해 클릭 이벤트 등록 (해당 플레이어의 투표를 기록)
            for (int i = 0; i < icp.buttons.Count; i++)
            {
                // 선택지는 1부터 시작 (첫 번째 버튼은 1, 두 번째 버튼은 2 등)
                int choiceVal = i + 1;
                Button btn = icp.buttons[i];
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Vote(player, choiceVal);
                });
            }

            // 플레이어별로 UI 패널 할당
            playerPanels[player] = icp;
        }

        // 모든 플레이어가 투표할 때까지 대기
        yield return new WaitUntil(() => playerVotes.Count >= totalPlayers);

        // 결과를 표시하기 위해 설정한 지연 시간만큼 대기
        yield return new WaitForSeconds(choiceDelay);

        // 모든 투표를 집계하여 최종 선택 결정 (동률 시 tieChoiceIndex 사용)
        finalVoteResult = CalculateMajorityWithTie(tieChoiceIndex);

        // 각 플레이어의 UI 패널을 최종 업데이트 후 비활성화
        foreach (var icp in playerPanels.Values)
        {
            // 최종 결과 반영
            UpdateVoteUI(icp);

            // 씬 내 오브젝트이므로 제거하지 않고 비활성화 처리
            icp.panel.SetActive(false);
        }
        playerPanels.Clear();

        // 최종 결과 콜백 호출 (다른 로직으로 최종 결과 전달)
        resultCallback?.Invoke(finalVoteResult);
    }

    // 특정 플레이어의 투표를 기록 (중복 투표는 무시)
    public void Vote(GameObject player, int choice)
    {
        if (player == null) return;

        // 이미 투표한 경우 무시
        if (playerVotes.ContainsKey(player)) return;

        // 플레이어의 투표 기록 추가
        playerVotes.Add(player, choice);

        // 만약 플레이어가 1명뿐이면 즉시 결과 처리
        if (totalPlayers == 1)
        {
            finalVoteResult = choice;
            UpdateVoteUI(playerPanels[player]);
            return;
        }

        // 각 플레이어의 UI 패널에 실시간 투표 현황 업데이트
        foreach (var icp in playerPanels.Values)
        {
            UpdateVoteUI(icp);
        }
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

        // 해당 선택지 반환
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
            if (playerPanels.Count > 0 && tiePick > playerPanels[new List<GameObject>(playerPanels.Keys)[0]].buttons.Count)
                tiePick = playerPanels[new List<GameObject>(playerPanels.Keys)[0]].buttons.Count;
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
