using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText; // UI Text 컴포넌트 연결
    private string logs = "";
    private int maxLines = 10; // 최대 표시할 줄 수
    
    public void Log(string message)
    {
        string[] lines = logs.Split('\n');
        if (lines.Length > maxLines)
        {
            logs = string.Join("\n", lines, 1, lines.Length - 1);
        }
        logs += $"\n{message}";
        debugText.text = logs;
    }

    // 일반 Debug.Log를 가로채서 UI에도 표시
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        Log($"[{type}] {logString}");
    }
}