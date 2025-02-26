using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Player Parts")]
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    [Networked] public int Token {get;set;}
    [Header("Foot Settings")]
    public float footOffset = 0.3f;
    public float footHeight = 0.1f;

    private HardwareRig _hardwareRig;

    public override void Spawned()
    {
        if (Object.HasInputAuthority) 
        {
            _hardwareRig = FindObjectOfType<HardwareRig>();
            if (_hardwareRig == null)
                Debug.LogError("HardwareRig를 찾을 수 없습니다!");
        }
    }
}
