using System;
using Fusion;
using UnityEngine;

[Serializable]
public struct HandCommand : INetworkStruct
{
    public int poseCommand;
}

public class HardwareHand : MonoBehaviour
{
    public RigPart side;
    public NetworkTransform networkTransform;
    public HandCommand handCommand;

    public int handPose = 0;
    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }

    protected void Update()
    {
        handCommand.poseCommand = handPose;
    }
}
