using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 시작 시 페이드 인 적용
public class SceneFadeInStart : MonoBehaviour
{
    void Start()
    { 
        StartCoroutine(FadeIn());
    }

    // 씬 로드 완료 후 페이드 인 실행
    private IEnumerator FadeIn()
    {
        // 씬 로드 여부 체크
        bool sceneLoaded = false;

        // 씬 로드 완료 이벤트
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 이벤트 호출 시 처리 내용
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬이 로드 -> OnSceneLoaded 함수 호출
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 이미 씬이 로드된 상태라면 바로 처리
        if (SceneManager.GetActiveScene().isLoaded)
        {
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬 로드 완료까지 대기
        yield return new WaitUntil(() => sceneLoaded);

        // 페이드 인 실행 
        StartCoroutine(FadeInOut.Instance.FadeIn());
    }
}
