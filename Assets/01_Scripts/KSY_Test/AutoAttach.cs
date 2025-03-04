using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(XRGrabInteractable))]
public class AutoAttach : MonoBehaviour
{
    [Header("부착 설정")]
    public Transform attachPoint;
    [TagSelector]
    public string attachPointTag;

    private XRGrabInteractable grabInteractable;

    // AttachPoint 안에 있는지 여부 (OnTriggerEnter/Exit로 업데이트)
    private bool isInAttachZone = false;

    // 현재 attachPoint에 부착되어 있는지 여부
    private bool isAttached = false;

    // 부착 전 Rigidbody의 kinematic 상태 저장
    private bool originalKinematic;

    // 충돌한 AttachPoint의 원래 색상을 저장하기 위한 Dictionary
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

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

        // 최초 씬에서 attachPoint를 태그로 할당
        AssignAttachPoint();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 전환 시 attachPoint 재참조
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignAttachPoint();
    }

    // attachPointTag를 이용해 attachPoint 할당
    private void AssignAttachPoint()
    {
        if (!string.IsNullOrEmpty(attachPointTag))
        {
            GameObject attachObj = GameObject.FindGameObjectWithTag(attachPointTag);
            if (attachObj != null)
            {
                attachPoint = attachObj.transform;
                Debug.Log("현재 씬에서 attachPoint가 할당되었습니다. 태그: " + attachPointTag);
            }
            else
            {
                Debug.LogWarning("태그 '" + attachPointTag + "'를 가진 attachPoint 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    // AttachPoint에 진입 시 true로 설정 및 색상 변경 (검은색)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttachPoint"))
        {
            isInAttachZone = true;
            Debug.Log("AttachPoint에 진입했습니다.");

            Renderer rend = other.GetComponent<Renderer>();
            if (rend != null)
            {
                // 원래 색상이 저장되지 않았다면 저장합니다.
                if (!originalColors.ContainsKey(other.gameObject))
                {
                    originalColors.Add(other.gameObject, rend.material.color);
                }
                rend.material.color = Color.black;
            }
            else
            {
                Debug.LogWarning("AttachPoint 오브젝트에 Renderer가 없습니다.");
            }
        }
    }

    // AttachPoint를 벗어날 때 원래 색상으로 복구
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AttachPoint"))
        {
            if (grabInteractable != null && (grabInteractable.isSelected || isAttached))
            {
                isInAttachZone = false;
                return;
            }

            Debug.Log("AttachPoint를 벗어났습니다.");
            isInAttachZone = false;

            Renderer rend = other.GetComponent<Renderer>();
            if (rend != null)
            {
                // 저장된 원래 색상이 있다면 복구합니다.
                if (originalColors.ContainsKey(other.gameObject))
                {
                    rend.material.color = originalColors[other.gameObject];
                    originalColors.Remove(other.gameObject);
                }
            }
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

    // AttachPoint 내에서 놓이면 attachPoint 위치로 부착(자식 설정)
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

        Debug.Log("AttachPoint 내에 놓여 attachPoint에 부착합니다.");

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

    // AttachPoint 밖에서 놓이면 씬의 최상위 계층으로 이동
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

        Debug.Log("AttachPoint 밖에 놓여, 상속 관계를 해제하였습니다.");
    }
}
