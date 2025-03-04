using System.Collections.Generic;
using UnityEngine;

// 플레이어와 초기 위치 정보를 함께 저장
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

    // 태그가 플레이어인 오브젝트를 찾아 빈 슬롯에 추가
    private void FillPlayersFromTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
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
