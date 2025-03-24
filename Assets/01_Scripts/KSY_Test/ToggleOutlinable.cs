using UnityEngine;
using EPOOutline;

public class ToggleOutlinable : MonoBehaviour
{
    [Header("Outlinable 활성화 여부")]
    [SerializeField, HideInInspector]
    private bool outlinableEnabled = false;

    // 외부에서 접근 가능한 프로퍼티
    public bool OutlinableEnabled
    {
        get => outlinableEnabled;
        set
        {
            outlinableEnabled = value;
            UpdateOutlinable();
        }
    }

    // 다른 스크립트에서 호출하여 켜짐/꺼짐 전환
    public void Toggle()
    {
        Outlinable comp = EnsureOutlinableComponent();
        comp.enabled = !comp.enabled;
        outlinableEnabled = comp.enabled;
        Debug.Log("Outlinable 컴포넌트 " + (comp.enabled ? "켜짐" : "꺼짐"));
    }

    // outlinableEnabled 값이 true이면 Outlinable 컴포넌트를 켜고,
    // false이면 Outlinable 컴포넌트를 끄는 역할
    private void UpdateOutlinable()
    {
        Outlinable comp = EnsureOutlinableComponent();
        comp.enabled = outlinableEnabled;
        Debug.Log("Outlinable 컴포넌트 " + (outlinableEnabled ? "켜짐" : "꺼짐"));
    }

    // Outlinable 컴포넌트가 없으면 추가
    private Outlinable EnsureOutlinableComponent()
    {
        Outlinable comp = GetComponent<Outlinable>();
        if (comp == null)
        {
            comp = gameObject.AddComponent<Outlinable>();
        }
        return comp;
    }
}
