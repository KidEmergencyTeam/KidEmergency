using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : SingletonManager<FadeInOut>
{
    [SerializeField] private float fadedTime = 0.5f; // FadeIn, FadeOut �� �ҿ� �ð�
    [SerializeField] private Image fadeInoutImg; // ���̵� ȿ���� �� �̹���

    public bool isFadeIn; // ���̵� �� ���� Ȯ��
    public bool isFadeOut; // ���̵� �ƿ� ���� Ȯ��

    void Start()
    {
        if (fadeInoutImg != null)
        {
            // �ʱ� ���� �� ���� (������ ���̰ų� �����ϰ�)
            Color color = fadeInoutImg.color;
            color.a = 0f; // ������ ���� (0)���� ����
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
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadedTime); // ���� ���� ���������� ����
            fadeInoutImg.color = color;
            yield return null;
        }

        color.a = 0f; // ������ ����
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
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadedTime); // ���� ���� ���������� ����
            fadeInoutImg.color = color;
            yield return null;
        }

        color.a = 1f; // ������ ������
        fadeInoutImg.color = color;
        isFadeOut = false;
    }
}