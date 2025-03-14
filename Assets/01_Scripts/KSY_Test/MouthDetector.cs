using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Collections;

public class MouthDetector : MonoBehaviour
{
    [Header("손수건 태그")]
    public string handkerTag = "Handker";

    [Header("전환할 씬 이름")]
    public string nextScene = "NextScene";

    [Header("인식 범위 (미터 단위)")]
    public float detectionRadius = 0.2f;

    [Header("손수건 XRGrabInteractable / 자동 처리됨")]
    public XRGrabInteractable lastGrabbedInteractable;

    // 충돌 여부 체크
    private bool triggered = false;

    // 입 오브젝트에 부착된 Collider는 Trigger로 설정되어 있어야 합니다.
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // 충돌한 오브젝트가 Handker 태그를 가지고 있는지 검사
        if (other.CompareTag(handkerTag))
        {
            // 충돌한 오브젝트에서 XRGrabInteractable.cs를 가져와 잡힌 상태인지 확인
            XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                // 가져온 XRGrabInteractable.cs를 lastGrabbedInteractable 필드에 저장 (인스펙터에서 확인 가능)
                lastGrabbedInteractable = grab;

                if (grab.isSelected)
                {
                    triggered = true;
                    StartCoroutine(Transition());
                }
            }
        }
    }

    private IEnumerator Transition()
    {
        Debug.Log("손수건이 잡힌 상태에서 입에 접촉했습니다. 씬 전환 시작...");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextScene);
    }

    // Scene에서 인식 범위 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
