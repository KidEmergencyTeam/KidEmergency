using Fusion;

public class NetworkHand : NetworkBehaviour
{
    public NetworkTransform networkTransform;

    public RigPart side;
    private NetworkRig _rig;

    private void Awake()
    {
        _rig = GetComponentInParent<NetworkRig>();
        networkTransform = GetComponent<NetworkTransform>();
    }
}