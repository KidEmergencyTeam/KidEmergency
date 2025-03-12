using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OutlineObjectController : XRBaseInteractable
{
     private Outlinable _outlinable;
     private Color _originColor;

    private void Start()
    {
        _outlinable = GetComponent<Outlinable>();
        _originColor = _outlinable.OutlineParameters.Color;
    }
    

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        print("상호작용 가능");
        _outlinable.OutlineParameters.Color = Color.green;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        print("상호작용 가능 범위 벗어남");
        _outlinable.OutlineParameters.Color = _originColor;
    }
}
