using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerEntry
{
    // 플레이어 참조
    public GameObject player;

    // 초기 위치와 회전
    public Vector3 initialPosition;
    public Vector3 initialRotation;

    // 스텝14에서의 위치와 회전
    public Vector3 step14Position;
    public Vector3 step14Rotation;
}

public class PlayerPosition : MonoBehaviour
{
    [Header("플레이어 슬롯")]
    public List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    private void Update()
    {
        // 빈 슬롯이 있으면 태그로부터 플레이어를 찾아 추가
        if (HasEmptySlot())
        {
            FillPlayersFromTag();
        }
    }

    // 빈 슬롯 존재 여부 확인
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

    // 태그 "Player"를 가진 오브젝트들을 찾아 슬롯에 추가
    private void FillPlayersFromTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        var sortedPlayers = players.OrderBy(p => p.transform.GetSiblingIndex()).ToArray();

        foreach (GameObject player in sortedPlayers)
        {
            AddPlayer(player);
        }
    }

    // 새로운 플레이어 추가: 초기 위치와 회전값 적용
    public void AddPlayer(GameObject newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("추가하려는 플레이어가 null입니다.");
            return;
        }

        // 이미 할당된 플레이어인지 확인
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

    // 모든 플레이어를 초기 위치와 회전으로 재배치
    public void ApplyInitialPositions()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.player != null)
            {
                entry.player.transform.position = entry.initialPosition;
                entry.player.transform.rotation = Quaternion.Euler(entry.initialRotation);
            }
        }
        Debug.Log("초기 위치 적용 완료");
    }

    // 모든 플레이어를 스텝14 위치와 회전으로 이동
    // ScenarioManager.cs에서 호출될 때만 스텝14 위치로 이동
    public void ApplyStep14Positions()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.player != null)
            {
                entry.player.transform.position = entry.step14Position;
                entry.player.transform.rotation = Quaternion.Euler(entry.step14Rotation);
            }
        }
        Debug.Log("스텝14 위치 적용 완료");
    }
}
