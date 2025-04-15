using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Fr_School_1,
    None,
    Fr_School_2,
    Fr_School_3,
    Fr_School_4
}
public enum UIType
{
    OptionPos,
    DialogPos,
    WarningPos,
    Seti
}

[System.Serializable]
public class UIData
{
    [Header("UI 오브젝트")]
    public GameObject uiObject;

    [Header("씬 이름")]
    public SceneName sceneName;

    [Header("UI 타입")]
    public UIType uiType;

    [Header("위치값")]
    public List<Vector3> changePositions = new List<Vector3>();

    [Header("회전값")]
    public List<Vector3> changeRotations = new List<Vector3>();

    [Header("true: 로컬 좌표 기준, false: 월드 좌표 기준")]
    public bool useLocal;

    // index에 해당하는 위치값 반환
    public Vector3 GetPosition(int index)
    {
        if (changePositions != null && index >= 0 && index < changePositions.Count)
        {
            return changePositions[index];
        }
        Debug.LogWarning(uiType + "의 changePositions 리스트에서 " + index + "번째 값을 찾을 수 없습니다.");
        return Vector3.zero;
    }

    // index에 해당하는 회전값 반환
    public Vector3 GetRotation(int index)
    {
        if (changeRotations != null && index >= 0 && index < changeRotations.Count)
        {
            return changeRotations[index];
        }
        Debug.LogWarning(uiType + "의 changeRotations 리스트에서 " + index + "번째 값을 찾을 수 없습니다.");
        return Vector3.zero;
    }
}

public class UIPosition : MonoBehaviour
{
    [Header("상세 설정")]
    public List<UIData> uIDatas = new List<UIData>();

    void Start()
    {
        // 열거형 선택에 따라 인덱스 설정
        int sceneIndex = GetSceneIndex();

        // uiObject가 null이면, UIType에 해당하는 태그로 게임오브젝트 찾기
        foreach (UIData uIData in uIDatas)
        {
            if (uIData.uiObject == null)
            {
                GameObject foundObject = GameObject.FindGameObjectWithTag(uIData.uiType.ToString());
                if (foundObject != null)
                {
                    uIData.uiObject = foundObject;
                }
                else
                {
                    Debug.LogError(uIData.uiType.ToString() + " 태그를 가진 게임오브젝트를 찾을 수 없습니다.");
                }
            }
        }

        // 해당 인덱스에 설정된 위치 및 회전값 업데이트
        UpdatePosition(sceneIndex);
    }

    // 현재 활성화된 씬 이름을 기반으로 인덱스를 반환
    int GetSceneIndex()
    {
        // 현재 활성화된 씬 이름 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;
        try
        {
            // 열거형에서 현재 씬 이름과 일치하는 값을 찾음
            SceneName scene = (SceneName)Enum.Parse(typeof(SceneName), currentSceneName);

            // 열거형 값을 정수형으로 변환하여 반환
            return (int)scene;
        }
        catch (Exception)
        {
            // 기본 인덱스 0 반환
            Debug.LogError("현재 씬 이름 \"" + currentSceneName + "\"이 SceneName 열거형에 정의x 따라서 기본 인덱스 0 사용");
            return 0;
        }
    }

    // 각 UIData에 대해 index에 해당하는 위치 및 회전값을 적용
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
                Debug.LogError(uIData.uiType.ToString() + " -> uiObject가 null입니다.");
            }
        }
    }
}
