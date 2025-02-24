using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SceneButtonPair
{
    // 빌드 세팅에 등록된 씬 중에서 문자열로 씬을 찾아 로드 -> 에디터뿐만 아니라 빌드 게임에서도 로드가 가능
    public string sceneName;
    // 버튼을 찾기 위한 태그
    public string buttonTag;
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private List<SceneButtonPair> sceneButtonPairs;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("새로운 씬 로드됨: " + scene.name);
        SetupSceneButtons(scene);
    }

    private void SetupSceneButtons(Scene scene)
    {
        foreach (var pair in sceneButtonPairs)
        {
            // 비활성화된 버튼도 포함하여 모든 Button 컴포넌트 검색 (true 인자로 비활성 오브젝트 포함)
            Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);
            foreach (Button button in allButtons)
            {
                // 버튼이 원하는 태그를 가지고 있는지 확인
                if (button.gameObject.CompareTag(pair.buttonTag))
                {
                    if (!string.IsNullOrEmpty(pair.sceneName))
                    {
                        string sceneName = pair.sceneName;
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => LoadScene(sceneName));
                        Debug.Log("버튼 재참조 및 설정 완료: " + sceneName);
                    }
                    else
                    {
                        Debug.LogError("sceneName이 할당되지 않음: " + button.gameObject.name);
                    }
                }
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log("버튼 클릭됨 - " + sceneName + " 씬으로 전환합니다.");
        SceneManager.LoadScene(sceneName);
    }
}
