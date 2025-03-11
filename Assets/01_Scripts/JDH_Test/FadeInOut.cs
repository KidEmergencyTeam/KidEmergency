using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] private float fadedTime = 0.5f; // FadeIn, FadeOut 총 소요 시간
    [SerializeField] private Image fadeInoutImg; // 페이드 효과를 줄 이미지

    [SerializeField] private bool isFadeIn; // 페이드 인 상태 확인
    [SerializeField] private bool isFadeOut; // 페이드 아웃 상태 확인

    void Start()
    {
        if (fadeInoutImg != null)
        {
            // 초기 알파 값 설정 (완전히 보이거나 투명하게)
            Color color = fadeInoutImg.color;
            color.a = 0f; // 완전히 투명 (0)으로 설정
            fadeInoutImg.color = color;
        }
    }

    public IEnumerator FadeIn()
    {
        isFadeIn = true;
        float elapsedTime = 0f;
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadedTime); // 알파 값을 점진적으로 감소
            fadeInoutImg.color = color;
            yield return null;
        }

        color.a = 0f; // 완전히 투명
        fadeInoutImg.color = color;
        isFadeIn = false;
    }

    public IEnumerator FadeOut()
    {
        isFadeOut = true;
        float elapsedTime = 0f;
        Color color = fadeInoutImg.color;

        while (elapsedTime < fadedTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadedTime); // 알파 값을 점진적으로 증가
            fadeInoutImg.color = color;
            yield return null;
        }

        color.a = 1f; // 완전히 불투명
        fadeInoutImg.color = color;
        isFadeOut = false;
    }
}