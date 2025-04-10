using UnityEngine;

// 안전 유도선 깜빡이 켜는 스크립트
public class ToggleHighlighter : MonoBehaviour
{
    // 씬에 존재하는 모든 Highlighter 스크립트를 찾아서 색상을 변경하고 깜빡임을 활성화
    public void Toggle()
    {
        // "SafetyLine" 태그가 붙은 오브젝트 찾아서 배열에 담기
        GameObject[] objects = GameObject.FindGameObjectsWithTag("SafetyLine");

        if (objects.Length == 0)
        {
            Debug.LogWarning("오브젝트를 찾을 수 없습니다.");
            return;
        }

        // Highlighter 컴포넌트에 접근하여 설정 적용
        foreach (GameObject obj in objects)
        {
            Highlighter highlighter = obj.GetComponent<Highlighter>();
            if (highlighter != null)
            {
                highlighter.SetColor(Color.yellow);
                highlighter.isBlinking = true;
            }
            else
            {
                Debug.LogWarning("해당 게임 오브젝트에서 Highlighter 컴포넌트를 찾을 수 없습니다: " + obj.name);
            }
        }
    }
}
