using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeInStart : MonoBehaviour
{
    void Start()
    {
        // 씬 로드 및 페이드 인 효과를 순차적으로 실행하는 코루틴 시작
        StartCoroutine(FadeIn());
    }

    // FadeIn 효과가 완료된 후 시나리오를 실행
    private IEnumerator FadeIn()
    {
        // 씬 로드 여부 체크
        bool sceneLoaded = false;

        // 씬 로드 완료 이벤트 핸들러
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 이미 씬이 로드된 상태라면 바로 처리
        if (SceneManager.GetActiveScene().isLoaded)
        {
            sceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬 로드 완료까지 대기
        yield return new WaitUntil(() => sceneLoaded);

        // 페이드 인 효과 실행 및 완료 대기
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());
    }
}
