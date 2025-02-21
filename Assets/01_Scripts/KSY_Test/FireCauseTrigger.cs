using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireCauseTrigger : MonoBehaviour
{
    public string message = "";
    private Renderer objRenderer;  // 오브젝트 색상 변경을 위한 Renderer
    private Color originalColor;   // 원래 색상 저장
    public Color highlightColor = Color.red;  // 클릭 시 변경할 색상

    void Start()
    {
        objRenderer = GetComponent<Renderer>();  // Renderer 가져오기
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;  // 원래 색상 저장
        }
    }

    // VR 컨트롤러 또는 클릭 이벤트로 실행
    public void OnObjectClicked()
    {
        ShowMessage();
        ChangeObjectColor();  // 즉시 색상 변경 후 복구
    }

    // 메시지 출력
    void ShowMessage()
    {
        Debug.Log(message + gameObject.name);  // 콘솔 출력
        UIManager.instance.ShowHint(message + gameObject.name);  // UI 출력 (VR HUD)
    }

    // 색상 변경 즉시 실행 후 바로 복구
    void ChangeObjectColor()
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = highlightColor;  // 색상 변경
            objRenderer.material.color = originalColor;  // 즉시 원래 색으로 복구
        }
    }

    // [PC 테스트용] 마우스로 클릭하면 실행
    private void OnMouseDown()
    {
        OnObjectClicked();
    }
}
