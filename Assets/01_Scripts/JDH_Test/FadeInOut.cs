using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 화면을 점점 밝게(FadeIn) 혹은 점점 어둡게(FadeOut) 만드는 싱글톤 클래스
public class FadeInOut : SingletonManager<FadeInOut>
{
    [SerializeField] private float fadedTime = 1.5f; // 페이드 효과가 진행되는 시간 (초)
    [SerializeField] private Image fadeInoutImg; // 페이드 효과를 적용할 이미지 (UI 패널)

    public bool isFadeIn;  // 현재 페이드 인이 진행 중인지 여부
    public bool isFadeOut; // 현재 페이드 아웃이 진행 중인지 여부

    void Start()
    {
        if (fadeInoutImg != null)
        {
            // 초기 설정: 페이드 이미지의 알파 값을 0으로 설정하여 화면이 보이도록 함
            Color color = fadeInoutImg.color;
            color.a = 0f; // 투명하게 설정
            fadeInoutImg.color = color;
        }
    }

    // 화면을 점점 밝게 만드는 코루틴 (페이드 인 효과)
    public IEnumerator FadeIn()
    {
        isFadeIn = true; // 페이드 인 진행 중임을 표시
        float elapsedTime = 0f; // 경과 시간 초기화
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadedTime); // 1(불투명) → 0(투명)으로 변화
            fadeInoutImg.color = color; // 이미지의 알파 값 적용
            yield return null; // 다음 프레임까지 대기
        }

        color.a = 0f; // 완전히 투명하게 설정
        fadeInoutImg.color = color;
        isFadeIn = false; // 페이드 인 종료
    }

    // 화면을 점점 어둡게 만드는 코루틴 (페이드 아웃 효과)
    public IEnumerator FadeOut()
    {
        isFadeOut = true; // 페이드 아웃 진행 중임을 표시
        float elapsedTime = 0f; // 경과 시간 초기화
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadedTime); // 0(투명) → 1(불투명)으로 변화
            fadeInoutImg.color = color; // 이미지의 알파 값 적용
            yield return null; // 다음 프레임까지 대기
        }

        color.a = 1f; // 완전히 불투명하게 설정
        fadeInoutImg.color = color;
        isFadeOut = false; // 페이드 아웃 종료
    }

    public IEnumerator SceneFadeEffect(string sceneName) // 씬 전환용 페이드 효과
    {
        yield return StartCoroutine(FadeOut());
        AsyncOperation asyncChange = SceneManager.LoadSceneAsync(sceneName); 
        
        while (!asyncChange.isDone)
        { 
            yield return null;
        }
        
        yield return StartCoroutine(FadeIn());
        
    }
}
