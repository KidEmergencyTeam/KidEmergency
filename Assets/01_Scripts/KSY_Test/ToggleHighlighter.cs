using UnityEngine;

// 안전 유도선 깜빡이 켜는 스크립트
public class ToggleHighlighter : MonoBehaviour
{
    private Highlighter highlighter;

    private void Start()
    {
        ToggleHighlighter toggleHighlighter = GameObject.FindGameObjectWithTag("SafetyLine")?.GetComponent<ToggleHighlighter>();

        if (toggleHighlighter == null)
        {
            Debug.LogError("ToggleOutlinable 컴포넌트를 찾을 수 없습니다.");
        }
    }

    // Highlighter 켜기
    public void Toggle()
    {
        highlighter.SetColor(Color.yellow);
        highlighter.isBlinking = true;
    }
}
