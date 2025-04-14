using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIData
{
    [Header("오브젝트 이름(디버그용)")]
    public string objectName;

    [Header("UI 및 세티")]
    public GameObject uiObject;

    [Header("위치값")]
    public List<Vector3> changePositions = new List<Vector3>();

    [Header("회전값")]
    public List<Vector3> changeRotations = new List<Vector3>();

    [Header("true: 로컬 좌표 기준, false: 월드 좌표 기준")]
    public bool useLocal;

    // index에 해당하는 위치값을 반환
    public Vector3 GetPosition(int index)
    {
        if (changePositions != null && index >= 0 && index < changePositions.Count)
        {
            return changePositions[index];
        }
        Debug.LogWarning(objectName + "의 changePositions 리스트 " + index + "번 index -> null");
        return Vector3.zero;
    }

    // index에 해당하는 회전값을 반환
    public Vector3 GetRotation(int index)
    {
        if (changeRotations != null && index >= 0 && index < changeRotations.Count)
        {
            return changeRotations[index];
        }
        Debug.LogWarning(objectName + "의 changeRotations 리스트 " + index + "번 index -> null.");
        return Vector3.zero;
    }
}

public class UIPosition : MonoBehaviour
{
    [Header("상세 설정")]
    public List<UIData> uIDatas = new List<UIData>();

    // 다른 스크립트에서 호출 시 입력한 매개변수에 따라 위치 및 회전값을 반영
    public void UpdatePosition(int index)
    {
        foreach (UIData uIData in uIDatas)
        {
            // index에 해당하는 오브젝트가 존재하면 실행
            if (uIData.uiObject != null)
            {
                // 대상 오브젝트 위치값 설정
                Vector3 targetPosition = uIData.GetPosition(index);

                // 대상 오브젝트 회전값 설정
                Vector3 targetRotation = uIData.GetRotation(index);

                // 로컬 좌표 기준
                if (uIData.useLocal)
                {
                    // 위치값 반영
                    uIData.uiObject.transform.localPosition = targetPosition;

                    // 회전값 반영
                    uIData.uiObject.transform.localRotation = Quaternion.Euler(targetRotation);
                }

                // 월드 좌표 기준
                else
                {
                    // 위치값 반영
                    uIData.uiObject.transform.position = targetPosition;

                    // 회전값 반영
                    uIData.uiObject.transform.rotation = Quaternion.Euler(targetRotation);
                }
            }
            else
            {
                Debug.LogError(uIData.objectName + " -> null");
            }
        }
    }
}
