using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class AutoAttach : MonoBehaviour
{
    [Header("부착 설정")]
    [Tooltip("AttachZone 안에서 놓였을 때 오브젝트가 부착될 위치/회전")]
    public Transform attachPoint;

    private XRGrabInteractable grabInteractable;

    // AttachZone 안에 있는지 여부 (OnTriggerEnter/Exit로 업데이트)
    private bool isInAttachZone = false;

    // 현재 attachPoint에 부착되어 있는지 여부
    private bool isAttached = false;

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
        Debug.Log("새로운 오브젝트가 장착되었습니다.");
    }

    // 부착 상태 해제 (부모 해제, 물리 상태 복원)
    public void Detach()
    {
        Debug.Log("오브젝트 부착 해제");

        transform.SetParent(null, true);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = originalKinematic;
        }
        isAttached = false;
    }

    // AttachZone 밖에서 놓이면 씬의 최상위 계층으로 이동
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

        Debug.Log("AttachZone 밖에 놓여, 상속 관계를 해제하였습니다.");
    }
}
