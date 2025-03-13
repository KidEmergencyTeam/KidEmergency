using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabOutlineObject : XRGrabInteractable
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
        _outlinable.OutlineParameters.Color = Color.green;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        _outlinable.OutlineParameters.Color = _originColor;
    }
}
