using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerEntry
{
    // 플레이어
    public GameObject player;

    // 플레이어의 초기 위치
    public Vector3 initialPosition;
}

// Step14 등에서 사용할 플레이어의 이동 목적지를 리스트로 정의 -> 학교 고급에서 문 앞으로 정렬
// PlayerEntry 리스트에 배치된 순서대로 리스트에 플레이어 재배치
// 따라서 스텝14 위치를 리스트로 정의할 때 플레이어를 묶어서 정의X 
[System.Serializable]
public class Destination
{
    // 이동 목적지 위치
    public Vector3 position;

    // 이동 목적지 로테이션 
    public Vector3 rotation;
}

public class PlayerPosition : MonoBehaviour
{
    [Header("초기 위치")]
    public List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    [Header("스텝14 위치")]
    public List<Destination> destinationPositions = new List<Destination>();

    private void Update()
    {
        // 빈 슬롯이 있으면 플레이어 검색 (매번 검색하지 않도록 조건부 호출)
        if (HasEmptySlot())
        {
            FillPlayersFromTag();
        }
    }

    // 빈 슬롯이 있는지 확인 (한마디로 모든 슬롯이 채워져 있지 않으면 true)
    private bool HasEmptySlot()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.player == null)
            {
                return true;
            }
        }
        // 모든 슬롯이 채워져 있다는 뜻입니다.
        return false;
    }

    // 태그가 "Player"인 오브젝트들을 찾아 슬롯에 추가
    private void FillPlayersFromTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        var sortedPlayers = players.OrderBy(p => p.transform.GetSiblingIndex()).ToArray();

        foreach (GameObject player in sortedPlayers)
        {
            AddPlayer(player);
        }
    }

    // 플레이어 추가 및 초기 위치로 배치
    public void AddPlayer(GameObject newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("추가하려는 플레이어가 null입니다.");
            return;
        }

        if (playerEntries.Exists(entry => entry.player == newPlayer))
        {
            return;
        }

        PlayerEntry freeEntry = playerEntries.Find(entry => entry.player == null);
        if (freeEntry != null)
        {
            freeEntry.player = newPlayer;
            newPlayer.transform.position = freeEntry.initialPosition;
            Debug.Log($"플레이어 추가됨: {newPlayer.name}");
        }
        else
        {
            Debug.LogWarning($"슬롯 부족: 플레이어 추가 불가 ({newPlayer.name})");
        }
    }
}
