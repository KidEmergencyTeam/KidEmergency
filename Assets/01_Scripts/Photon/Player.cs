using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Player Parts")]
    public Transform headTransform;
    public Transform bodyTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform leftFootTransform;
    public Transform rightFootTransform;

    [Header("Foot Settings")]
    public float footOffset = 0.3f;
    public float footHeight = 0.1f;

    [Networked] 
    private NetworkButtons PreviousButtons { get; set; }

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

    public override void FixedUpdateNetwork()
    {
        if (GetInput<RigInput>(out var input))
        {
            // 헤드셋과 컨트롤러 업데이트
            headTransform.position = input.headPosition;
            headTransform.rotation = input.headRotation;

            leftHandTransform.position = input.leftHandPosition;
            leftHandTransform.rotation = input.leftHandRotation;
            rightHandTransform.position = input.rightHandPosition;
            rightHandTransform.rotation = input.rightHandRotation;

            // 몸통 위치 업데이트
            bodyTransform.position = input.headPosition + Vector3.down * 0.5f;
            bodyTransform.rotation = Quaternion.Euler(0, input.headRotation.eulerAngles.y, 0);

            // 발 위치 자동 업데이트
            Vector3 bodyForward = bodyTransform.forward;
            Vector3 bodyRight = bodyTransform.right;

            // 왼발 위치
            leftFootTransform.position = bodyTransform.position + (-bodyRight * footOffset) + Vector3.down * footHeight;
            leftFootTransform.rotation = bodyTransform.rotation;

            // 오른발 위치
            rightFootTransform.position = bodyTransform.position + (bodyRight * footOffset) + Vector3.down * footHeight;
            rightFootTransform.rotation = bodyTransform.rotation;
        }
    }
}
