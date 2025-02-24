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
            // 기본적으로 Throw On Detach를 활성화한 상태로 둡니다.
            grabInteractable.throwOnDetach = true;
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
            // 오브젝트가 잡혀 있거나 부착 중이면 로그 출력하지 않음
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

        if (attachPoint.childCount > 0 && attachPoint.GetChild(0) != transform)
        {
            Debug.Log("이미 부착된 오브젝트가 있습니다. 기존 오브젝트를 해제합니다.");

            Transform previous = attachPoint.GetChild(0);
            AutoAttach previousAutoAttach = previous.GetComponent<AutoAttach>();
            if (previousAutoAttach != null)
            {
                previousAutoAttach.Detach();
            }
            else
            {
                previous.SetParent(null, true);
            }
        }

        if (isAttached)
            return;

        Debug.Log("AttachZone 내에 놓여 attachPoint에 부착합니다.");

        // attachPoint 영역에 배치할 때는 Throw On Detach 기능을 비활성화
        if (grabInteractable != null)
            grabInteractable.throwOnDetach = false;

        Vector3 worldScaleBefore = transform.lossyScale;
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }

        transform.SetParent(attachPoint, true);

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
        AddProxyCollider();
        Debug.Log("새로운 오브젝트가 장착되었습니다.");
    }

    // 부착 상태 해제 (부모 해제, 물리 상태 복원, 프록시 제거)
    public void Detach()
    {
        Debug.Log("오브젝트 부착 해제");

        transform.SetParent(null, true);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = originalKinematic;
        }

        Transform proxy = transform.Find("AttachPointProxyCollider");
        if (proxy != null)
        {
            Destroy(proxy.gameObject);
        }
        proxyAdded = false;
        isAttached = false;
    }

    // AttachZone 밖에서 놓이면 씬의 최상위 계층(루트)으로 이동
    private void DetachToSceneRoot()
    {
        if (isAttached)
            Detach();

        // attachPoint 영역 외에서는 던지기 기능을 활성화합니다.
        if (grabInteractable != null)
            grabInteractable.throwOnDetach = true;

        transform.SetParent(null, true);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log("AttachZone 밖에 놓여, 씬 루트로 해제하였습니다.");
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
        boxCollider.size = new Vector3(proxyColliderSize, proxyColliderSize, proxyColliderSize);
        boxCollider.isTrigger = false;

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
