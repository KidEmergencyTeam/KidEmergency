using Fusion;
using UnityEngine;

public class HardwareHand : MonoBehaviour
{
    public RigPart side;
    public NetworkTransform networkTransform;

    public int handPose = 0;
    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }
}
