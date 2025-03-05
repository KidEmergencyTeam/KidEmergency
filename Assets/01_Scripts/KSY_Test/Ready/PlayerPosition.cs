using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

[System.Serializable]
public class PlayerEntry
{
    // 플레이어
    public GameObject player;

    // 플레이어의 초기 위치
    public Vector3 initialPosition;
}

public class PlayerPosition : MonoBehaviour
{
    public List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    private void Update()
    {
        // 빈 슬롯이 있으면 플레이어 검색
        if (HasEmptySlot())
        {
            FillPlayersFromTag();
        }
    }

    // 모든 슬롯이 채워져 있는지 확인
    private bool HasEmptySlot()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.player == null)
            {
                return true;
            }
        }
        return false;
    }

    // 플레이어 찾기 및 추가
    private void FillPlayersFromTag()
    {
        // 태그가 "Player"인 오브젝트들을 찾음
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // Hierarchy에 배치된 순서대로 정렬
        var sortedPlayers = players.OrderBy(p => p.transform.GetSiblingIndex()).ToArray();

        // 정렬된 순서대로 빈 슬롯에 플레이어를 추가
        foreach (GameObject player in sortedPlayers)
        {
            AddPlayer(player);
        }
    }

    // 플레이어 추가
    public void AddPlayer(GameObject newPlayer)
    {
        // 플레이어 유효성 체크
        if (newPlayer == null)
        {
            Debug.LogWarning("추가하려는 플레이어가 null입니다.");
            return;
        }

        // 이미 등록된 플레이어인지 확인
        if (playerEntries.Exists(entry => entry.player == newPlayer))
        {
            return;
        }

        // null인 슬롯 찾기
        PlayerEntry freeEntry = playerEntries.Find(entry => entry.player == null);
        if (freeEntry != null)
        {
            // 비어있는 슬롯이 있으면 플레이어 추가
            freeEntry.player = newPlayer;
            newPlayer.transform.position = freeEntry.initialPosition;
            Debug.Log($"플레이어 추가됨: {newPlayer.name}");
        }
        else
        {
            // 비어있는 슬롯이 없으면 플레이어 추가 불가 -> 인원 제한
            Debug.LogWarning($"슬롯 부족: 플레이어 추가 불가 ({newPlayer.name})");
        }
    }
}
