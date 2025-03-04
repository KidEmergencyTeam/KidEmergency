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

    private void Start()
    {
        // 설정한 초기 위치에서 플레이어 불러오기
        SetInitialPositionsOnce();
    }

    public void SetInitialPositionsOnce()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.player != null)
            {
                entry.player.transform.position = entry.initialPosition;
            }
            else
            {
                Debug.LogWarning($"초기 슬롯에 플레이어가 등록되어 있지 않습니다. (초기 위치: {entry.initialPosition})");
            }
        }
    }

    // 플레이어 추가
    public void AddPlayer(GameObject newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("추가하려는 플레이어가 null입니다.");
            return;
        }

        // 이미 등록된 플레이어인지 확인
        if (playerEntries.Exists(entry => entry.player == newPlayer))
        {
            Debug.LogWarning($"이미 리스트에 포함된 플레이어입니다: {newPlayer.name}");
            return;
        }

        // null인 슬롯(비어있는 슬롯) 찾기
        PlayerEntry freeEntry = playerEntries.Find(entry => entry.player == null);
        if (freeEntry != null)
        {
            // 비어있는 슬롯을 재활용
            freeEntry.player = newPlayer;
            newPlayer.transform.position = freeEntry.initialPosition;
            Debug.Log($"플레이어 추가됨 (재활용된 슬롯): {newPlayer.name}");
        }
        else
        {
            // 슬롯이 없으므로 플레이어 추가 불가 -> 인원 제한
            Debug.LogWarning($"슬롯 부족: 플레이어 추가 불가 ({newPlayer.name})");
        }
    }
}
