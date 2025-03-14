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

    // 플레이어의 초기 위치에서 적용할 로테이션
    public Vector3 initialRotation;
}

public class InitialPlayerPosition : MonoBehaviour
{
    [Header("초기 위치")]
    public List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    private void Update()
    {
        // 빈 슬롯이 있으면 플레이어 검색 (매번 검색하지 않도록 조건부 호출)
        if (HasEmptySlot())
        {
            FillPlayersFromTag();
        }
    }

    // 빈 슬롯이 있는지 확인 (모든 슬롯이 채워져 있지 않으면 true)
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

    // 플레이어 추가 및 초기 위치와 로테이션 값으로 배치
    public void AddPlayer(GameObject newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("추가하려는 플레이어가 null입니다.");
            return;
        }

        // 이미 해당 플레이어가 할당되어 있다면 무시
        if (playerEntries.Exists(entry => entry.player == newPlayer))
        {
            return;
        }

        // 빈 슬롯 찾기
        PlayerEntry freeEntry = playerEntries.Find(entry => entry.player == null);
        if (freeEntry != null)
        {
            freeEntry.player = newPlayer;
            newPlayer.transform.position = freeEntry.initialPosition;
            newPlayer.transform.rotation = Quaternion.Euler(freeEntry.initialRotation);
            Debug.Log($"플레이어 추가됨: {newPlayer.name}");
        }
        else
        {
            Debug.LogWarning($"슬롯 부족: 플레이어 추가 불가 ({newPlayer.name})");
        }
    }
}
