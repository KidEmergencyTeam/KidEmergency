using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireCauseTrigger : MonoBehaviour
{
    public string message = "";
    private Renderer objRenderer;
    private Color originalColor;
    public Color highlightColor = Color.white;

    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
        }
    }

    // Select Entered 이벤트 핸들러: SelectEnterEventArgs를 매개변수로 받음
    public void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        ShowMessage();
        ChangeObjectColor();
    }

    private void ShowMessage()
    {
        Debug.Log(message);
        UIManager.instance.ShowExplanation(message);
    }

    private void ChangeObjectColor()
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = highlightColor;
            Invoke("ResetColor", 5f);  // 5초 후 원래 색상 복구
        }
    }

    private void ResetColor()
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = originalColor;
        }
    }
}
