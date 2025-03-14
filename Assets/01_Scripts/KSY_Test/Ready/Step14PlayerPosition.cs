using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Destination
{
    // 이동 목적지 위치
    public Vector3 position;

    // 이동 목적지 로테이션
    public Vector3 rotation;
}

public class Step14PlayerPosition : MonoBehaviour
{
    [Header("스텝14 위치")]
    public List<Destination> destinationPositions = new List<Destination>();

    // 예시: 외부에서 플레이어 리스트를 전달 받아 순서대로 배치
    public void ApplyStep14Positions(List<GameObject> players)
    {
        int count = Mathf.Min(players.Count, destinationPositions.Count);
        for (int i = 0; i < count; i++)
        {
            if (players[i] != null)
            {
                players[i].transform.position = destinationPositions[i].position;
                players[i].transform.rotation = Quaternion.Euler(destinationPositions[i].rotation);
            }
        }
        Debug.Log("스텝14 위치 적용 완료");
    }

    // 또는 태그를 이용하여 플레이어를 직접 검색한 후 적용할 수도 있습니다.
    public void ApplyStep14PositionsFromTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // 예시로 SiblingIndex 기준 정렬 (필요에 따라 정렬 기준을 변경)
        var sortedPlayers = new List<GameObject>(players);
        sortedPlayers.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        int count = Mathf.Min(sortedPlayers.Count, destinationPositions.Count);
        for (int i = 0; i < count; i++)
        {
            if (sortedPlayers[i] != null)
            {
                sortedPlayers[i].transform.position = destinationPositions[i].position;
                sortedPlayers[i].transform.rotation = Quaternion.Euler(destinationPositions[i].rotation);
            }
        }
        Debug.Log("스텝14 위치 적용 완료 (태그 기준)");
    }
}
