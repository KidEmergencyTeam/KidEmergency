using UnityEngine;
using UnityEngine.UI;

// UI Image 컴포넌트를 FadeInOut.cs에 연결
public class FadeInOutImage : MonoBehaviour
{
    // Inspector에서 할당할 이미지 (Fade 패널의 Image 컴포넌트)
    [SerializeField] private Image fadeImage;

    void Start()
    {
        if (FadeInOut.Instance != null)
        {
            // FadeInOut.cs에 fadeImage 할당
            FadeInOut.Instance.SetFadeImage(fadeImage);
        }
        else
        {
            Debug.LogWarning("FadeInOut.cs가 존재하지 않습니다.");
        }
    }
}
