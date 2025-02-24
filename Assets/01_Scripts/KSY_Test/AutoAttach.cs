using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(XRGrabInteractable))]
public class AutoAttach : MonoBehaviour
{
    [Header("부착 설정")]
    [Tooltip("AttachZone 안에서 놓았을 때 오브젝트가 부착될 위치/회전")]
    public Transform attachPoint;

    [Tooltip("부착 후 XR 레이 인식을 위한 가상 콜라이더의 크기 (모든 축 동일 적용)")]
    public float proxyColliderSize = 0.5f;

    [Tooltip("XR Origin (XR Rig) 밖으로 이동시킬 부모 (없으면 씬 루트)")]
    public Transform sceneRootObject;

    private XRGrabInteractable grabInteractable;

    // AttachZone 안에 있는지 여부 (OnTriggerEnter/Exit로 업데이트)
    private bool isInAttachZone = false;

    // 현재 attachPoint에 부착되어 있는지 여부
    private bool isAttached = false;

    // 프록시 콜라이더를 한 번만 생성하기 위한 플래그
    private bool proxyAdded = false;

    // 부착 전 Rigidbody의 kinematic 상태 저장
    private bool originalKinematic;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
        else
        {
            Debug.LogError("XRGrabInteractable이 존재하지 않습니다!");
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    // AttachZone에 진입 시 true로 설정
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttachZone"))
        {
            isInAttachZone = true;
            Debug.Log("AttachZone에 진입했습니다.");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AttachZone"))
        {
            // 만약 오브젝트가 잡혀 있거나(Select중) 부착 상태이면 로그 출력하지 않음
            if (grabInteractable != null && (grabInteractable.isSelected || isAttached))
            {
                isInAttachZone = false;
                return;
            }

            Debug.Log("AttachZone을 벗어났습니다.");
            isInAttachZone = false;
        }
    }

    // 오브젝트를 잡을 때 호출
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("오브젝트를 잡았습니다.");

        // 이미 부착되어 있다면(attachPoint의 자식 상태라면) 먼저 해제
        if (isAttached)
        {
            Detach();
        }
    }

    // 오브젝트를 놓을 때 호출
    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.isCanceled)
            return;

        // 놓는 순간, AttachZone 내에 있으면 부착, 그렇지 않으면 XR Origin 밖으로 해제
        if (isInAttachZone)
        {
            Attach();
        }
        else
        {
            DetachToSceneRoot();
        }
    }

    // AttachZone 내에서 놓이면 attachPoint 위치로 부착(자식 설정)
    private void Attach()
    {
        if (attachPoint == null)
        {
            Debug.LogWarning("attachPoint가 설정되지 않아 부착할 수 없습니다.");
            return;
        }

        // 이미 부착 중이면 중복 처리하지 않음
        if (isAttached)
            return;

        Debug.Log("AttachZone 내에 놓여 attachPoint에 부착합니다.");

        // 월드 스케일 보존을 위해 현재 스케일 저장
        Vector3 worldScaleBefore = transform.lossyScale;

        // attachPoint의 위치와 회전으로 이동
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;

        // Rigidbody의 상태 변경: 물리 영향을 제거하기 위해 kinematic으로 전환
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }

        // attachPoint를 부모로 설정 (worldPositionStays 기본값 true)
        transform.SetParent(attachPoint, true);

        // 부모의 스케일 영향을 제거하여 원래 월드 스케일 유지
        Vector3 parentScale = attachPoint.lossyScale;
        if (parentScale.x != 0 && parentScale.y != 0 && parentScale.z != 0)
        {
            transform.localScale = new Vector3(
                worldScaleBefore.x / parentScale.x,
                worldScaleBefore.y / parentScale.y,
                worldScaleBefore.z / parentScale.z
            );
        }
        else
        {
            Debug.LogWarning("attachPoint의 스케일 값이 0입니다. 스케일 조정이 제대로 되지 않습니다.");
        }

        isAttached = true;

        // XR Ray Interactor가 인식할 수 있도록 프록시 콜라이더 추가 (한번만 추가)
        AddProxyCollider();
    }

    // 부착 상태 해제 (부모 해제, 물리 상태 복원, 프록시 제거)
    private void Detach()
    {
        Debug.Log("오브젝트 부착 해제");

        // 부모 관계 해제 (worldPositionStays true로 하여 위치 유지)
        transform.SetParent(null, true);

        // Rigidbody 물리 상태 복원
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = originalKinematic;
        }

        // 프록시 콜라이더 제거
        Transform proxy = transform.Find("AttachPointProxyCollider");
        if (proxy != null)
        {
            Destroy(proxy.gameObject);
        }
        proxyAdded = false;

        isAttached = false;
    }

    // AttachZone 밖에서 놓이면 XR Origin(또는 sceneRootObject) 밖으로 이동
    private void DetachToSceneRoot()
    {
        // 만약 부착되어 있다면 먼저 해제
        if (isAttached)
            Detach();

        // sceneRootObject가 지정되어 있으면 그 아래로, 없으면 씬 루트로 이동
        if (sceneRootObject != null)
        {
            transform.SetParent(sceneRootObject, true);
        }
        else
        {
            transform.SetParent(null, true);
        }

        // Rigidbody 물리 재활성 (자유롭게 움직이도록)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log("AttachZone 밖에 놓여, XR Origin 계층에서 해제하였습니다.");
    }

    // XR Ray Interactor가 인식하기 쉬운 프록시 박스 콜라이더 추가
    private void AddProxyCollider()
    {
        if (proxyAdded)
            return;

        GameObject proxy = new GameObject("AttachPointProxyCollider");
        proxy.transform.SetParent(transform, false);
        proxy.transform.localPosition = Vector3.zero;
        proxy.transform.localRotation = Quaternion.identity;

        BoxCollider boxCollider = proxy.AddComponent<BoxCollider>();
        // 모든 축 동일 크기 적용
        boxCollider.size = new Vector3(proxyColliderSize, proxyColliderSize, proxyColliderSize);

        // 프로젝트 설정에 맞게 trigger 여부 결정 (일반적으로 XR Ray Interactor는 non-trigger를 감지)
        boxCollider.isTrigger = false;

        // grabInteractable의 콜라이더 목록을 갱신 (필요 시 기존 콜라이더 삭제)
        if (grabInteractable != null)
        {
            grabInteractable.colliders.Clear();
            grabInteractable.colliders.Add(boxCollider);
        }
        else
        {
            Debug.LogWarning("XRGrabInteractable이 null입니다. 프록시 콜라이더 등록 실패!");
        }

        proxyAdded = true;
        Debug.Log("AttachPoint 프록시 박스 콜라이더 추가");
    }

    // 씬 뷰에서 프록시 콜라이더 크기 시각화 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        if (attachPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(attachPoint.position,
                new Vector3(proxyColliderSize, proxyColliderSize, proxyColliderSize));

#if UNITY_EDITOR
            Handles.Label(attachPoint.position + Vector3.up * proxyColliderSize,
                          "Size: " + proxyColliderSize.ToString("F2"));
#endif
        }
    }
}
