using UnityEngine;
using UnityEngine.UI;

// fadeInoutImg 전달
public class SendFadeImage : MonoBehaviour
{
    [Header("페이드 인 아웃 이미지")]
    [SerializeField] private Image fadeInoutImg;

    private void Start()
    {
        if (fadeInoutImg == null)
        {
            Debug.LogError("[SendFadeImage] 페이드 인 아웃 이미지가 할당되지 않음");
            return;
        }

        if (FadeInOut.Instance != null)
        {
            FadeInOut.Instance.SetFadeImage(fadeInoutImg);
        }
        else
        {
            Debug.LogError("[SendFadeImage] FadeInOut 인스턴스를 찾을 수 없음");
        }
    }
}
