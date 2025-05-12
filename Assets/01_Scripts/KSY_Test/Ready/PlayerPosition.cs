using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 위치 설정
[System.Serializable]
public class PlayerEntry
{
    [Header("플레이어 및 NPC")]
    public GameObject playerNpc;

    [Header("초기 위치와 회전")]
    public Vector3 initialPosition;
    public Vector3 initialRotation;
}

public class PlayerPosition : MonoBehaviour
{
    [Header("플레이어 슬롯")]
    public List<PlayerEntry> playerEntries = new List<PlayerEntry>();

    private void Update()
    {
        // 빈 슬롯이 있으면 태그로부터 플레이어와 NPC를 찾아 추가
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
            if (entry.playerNpc == null)
            {
                return true;
            }
        }
        return false;
    }

    // 태그 "Player"와 "NPC"를 가진 오브젝트들을 찾아 슬롯에 추가
    private void FillPlayersFromTag()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] npcObjects = GameObject.FindGameObjectsWithTag("NPC");

        // 두 배열을 합쳐서 모두 처리
        List<GameObject> allObjects = new List<GameObject>();
        allObjects.AddRange(playerObjects);
        allObjects.AddRange(npcObjects);

        // 필요에 따라 정렬 (여기서는 GetSiblingIndex 기준 정렬)
        var sortedObjects = allObjects.OrderBy(p => p.transform.GetSiblingIndex()).ToArray();

        foreach (GameObject obj in sortedObjects)
        {
            AddPlayer(obj);
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
        if (playerEntries.Exists(entry => entry.playerNpc == newPlayer))
        {
            return;
        }

        // 빈 슬롯 찾기
        PlayerEntry freeEntry = playerEntries.Find(entry => entry.playerNpc == null);
        if (freeEntry != null)
        {
            freeEntry.playerNpc = newPlayer;
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
            if (entry.playerNpc != null)
            {
                entry.playerNpc.transform.position = entry.initialPosition;
                entry.playerNpc.transform.rotation = Quaternion.Euler(entry.initialRotation);
            }
        }
        Debug.Log("초기 위치 적용 완료");
    }
}
