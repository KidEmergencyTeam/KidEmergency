using Fusion;
using UnityEngine;

public class NetworkRig : NetworkBehaviour
{
    public HardwareRig hardwareRig;
    public NetworkHand leftHand;
    public NetworkHand rightHand;
    public NetworkHeadset headset;
    
    [HideInInspector] public NetworkTransform networkTransform;

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }

    public override void Spawned()
    {
        base.Spawned();
        hardwareRig = FindObjectOfType<HardwareRig>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<RigInput>(out var input))
        {
            transform.position = input.playAreaPosition;
            transform.rotation = input.playAreaRotation;
            leftHand.transform.position = input.leftHandPosition;
            leftHand.transform.rotation = input.leftHandRotation;
            rightHand.transform.position = input.rightHandPosition;
            rightHand.transform.rotation = input.rightHandRotation;
            headset.transform.position = input.headPosition;
            headset.transform.rotation = input.headRotation;
        }
    }

    public bool IsLocalNetworkRig => Object.HasInputAuthority;

    public override void Render()
    {
        base.Render();
        if (IsLocalNetworkRig)
        {
            transform.position = hardwareRig.transform.position;
            transform.rotation = hardwareRig.transform.rotation;
            leftHand.transform.position = hardwareRig.leftController.transform.position;
            leftHand.transform.rotation = hardwareRig.leftController.transform.rotation;
            rightHand.transform.position = hardwareRig.rightController.transform.position;
            rightHand.transform.rotation = hardwareRig.rightController.transform.rotation;
            headset.transform.position = hardwareRig.headset.transform.position;
            headset.transform.rotation = hardwareRig.headset.transform.rotation;
        }
    }
}
