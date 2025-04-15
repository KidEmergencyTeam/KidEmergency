using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    None,
    Fr_School_1,
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
public class SceneTransformMapping
{
    [Header("씬 이름")]
    public SceneName scene;

    [Header("위치값")]
    public Vector3 position;

    [Header("회전값")]
    public Vector3 rotation;
}

[System.Serializable]
public class UIData
{
    [Header("UI 오브젝트")]
    public GameObject uiObject;

    [Header("UI 타입")]
    public UIType uiType;

    [Header("위치/회전값 (씬별)")]
    public List<SceneTransformMapping> sceneTransforms = new List<SceneTransformMapping>();

    [Header("true: 로컬 좌표 기준, false: 월드 좌표 기준")]
    public bool useLocal;

    // 전달된 씬에 해당하는 위치값 반환
    public Vector3 GetPosition(SceneName currentScene)
    {
        foreach (SceneTransformMapping mapping in sceneTransforms)
        {
            if (mapping.scene.Equals(currentScene))
            {
                return mapping.position;
            }
        }
        Debug.LogWarning(uiType + "의 sceneTransforms 리스트에 " + currentScene + "에 해당하는 위치값을 찾을 수 없습니다.");
        return Vector3.zero;
    }

    // 전달된 씬에 해당하는 회전값 반환
    public Vector3 GetRotation(SceneName currentScene)
    {
        foreach (SceneTransformMapping mapping in sceneTransforms)
        {
            if (mapping.scene.Equals(currentScene))
            {
                return mapping.rotation;
            }
        }
        Debug.LogWarning(uiType + "의 sceneTransforms 리스트에 " + currentScene + "에 해당하는 회전값을 찾을 수 없습니다.");
        return Vector3.zero;
    }
}

public class UIPosition : MonoBehaviour
{
    [Header("상세 설정")]
    public List<UIData> uIDatas = new List<UIData>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 활성화된 씬의 이름을 SceneName으로 변환
        SceneName currentScene;
        try
        {
            currentScene = (SceneName)Enum.Parse(typeof(SceneName), SceneManager.GetActiveScene().name);
        }
        catch (Exception)
        {
            Debug.LogError("현재 씬 이름 \"" + SceneManager.GetActiveScene().name + "\"이 SceneName 열거형에 정의되어 있지 않습니다. 기본값 None 사용");
            currentScene = SceneName.None;
        }

        // uiObject가 null이면, UIType에 해당하는 태그로 오브젝트 찾기
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
                    Debug.LogError(uIData.uiType.ToString() + " 태그를 가진 오브젝트를 찾을 수 없습니다.");
                }
            }
        }

        // 해당 씬에 따른 위치 및 회전값 적용
        UpdatePosition(currentScene);
    }

    // 각 UIData에 대해 전달받은 씬에 해당하는 위치 및 회전값을 적용
    public void UpdatePosition(SceneName currentScene)
    {
        foreach (UIData uIData in uIDatas)
        {
            if (uIData.uiObject != null)
            {
                // 씬에 해당하는 위치값과 회전값 가져오기
                Vector3 targetPosition = uIData.GetPosition(currentScene);
                Vector3 targetRotation = uIData.GetRotation(currentScene);

                // 로컬 좌표 기준
                if (uIData.useLocal)
                {
                    uIData.uiObject.transform.localPosition = targetPosition;
                    uIData.uiObject.transform.localRotation = Quaternion.Euler(targetRotation);
                }
                // 월드 좌표 기준
                else
                {
                    uIData.uiObject.transform.position = targetPosition;
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
