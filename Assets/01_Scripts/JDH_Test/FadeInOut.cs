using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 화면을 점점 밝게(FadeIn) 혹은 점점 어둡게(FadeOut) 만드는 싱글톤 클래스
public class FadeInOut : SingletonManager<FadeInOut>
{
    // 페이드 효과가 진행되는 시간 
    [SerializeField] private float fadedTime = 1.5f;

    // 페이드 효과를 적용할 이미지 
    [SerializeField] private Image fadeInoutImg; 

    [Header("페이드 인 진행 여부")]
    public bool isFadeIn;  
    
    [Header("페이드 아웃 진행 여부")]
    public bool isFadeOut; 

    void Start()
    {
        if (fadeInoutImg != null)
        {
            // 초기 설정: 페이드 이미지의 알파 값을 0으로 설정하여 화면이 보이도록 함
            Color color = fadeInoutImg.color;
            // 투명하게 설정
            color.a = 0f; 
            fadeInoutImg.color = color;
        }
    }

    // 외부에서 이미지 할당 가능하도록 public 메서드 추가
    public void SetFadeImage(Image image)
    {
        fadeInoutImg = image;
        if (fadeInoutImg != null)
        {
            Color color = fadeInoutImg.color;
            color.a = 0f; 
            fadeInoutImg.color = color;
        }
        else
        {
            Debug.LogWarning("FadeInOut: 할당된 Image가 null입니다.");
        }
    }

    // 화면을 점점 밝게 만드는 코루틴 (페이드 인 효과)
    public IEnumerator FadeIn()
    {
        // 페이드 인 진행 중임을 표시
        isFadeIn = true;
        // 경과 시간 초기화
        float elapsedTime = 0f; 
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            // 1(불투명) → 0(투명)으로 변화
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadedTime);
            // 이미지의 알파 값 적용
            fadeInoutImg.color = color; 
            yield return null; 
        }

        // 완전히 투명하게 설정
        color.a = 0f; 
        fadeInoutImg.color = color;
        // 페이드 인 종료
        isFadeIn = false; 
    }

    // 화면을 점점 어둡게 만드는 코루틴 (페이드 아웃 효과)
    public IEnumerator FadeOut()
    {
        // 페이드 아웃 진행 중임을 표시
        isFadeOut = true;
        // 경과 시간 초기화
        float elapsedTime = 0f; 
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            // 0(투명) → 1(불투명)으로 변화
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadedTime);
            // 이미지의 알파 값 적용
            fadeInoutImg.color = color; 
            yield return null; 
        }

        // 완전히 불투명하게 설정
        color.a = 1f; 
        fadeInoutImg.color = color;
        // 페이드 아웃 종료
        isFadeOut = false; 
    }

    // 씬 전환용 페이드 효과
    public IEnumerator SceneFadeEffect(string sceneName) 
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
