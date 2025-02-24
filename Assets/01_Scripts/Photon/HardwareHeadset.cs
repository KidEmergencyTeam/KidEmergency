using Fusion;
using UnityEngine;

public class HardwareHeadset : MonoBehaviour
{
    public NetworkTransform networkTransform;

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }
}
