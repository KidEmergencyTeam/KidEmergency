using Fusion;
using UnityEngine;

public class HardwareHand : MonoBehaviour
{
    public RigPart side;
    public NetworkTransform networkTransform;

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }
}
