using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class MouthDetector : MonoBehaviour
{
    [Header("손수건 태그")]
    public string handkerTag = "Handker";

    [Header("인식 범위 (미터 단위)")]
    public float detectionRadius = 0.2f;

    [Header("손수건 XRGrabInteractable / 자동 처리됨")]
    public XRGrabInteractable lastGrabInteractable;

    // 이미 충돌한 경우 중복 처리를 막기 위한 플래그
    private bool triggered = false;

    // 입 오브젝트에 부착된 Collider는 Trigger로 설정
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // 충돌한 오브젝트가 handkerTag를 가지고 있는지 검사
        if (other.CompareTag(handkerTag))
        {
            // 충돌한 오브젝트의 XRGrabInteractable.cs를 가져오기
            XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                lastGrabInteractable = grab;
            }

            // 충돌 시 코루틴 실행
            triggered = true;
            StartCoroutine(HandleHandkerGrab());
        }
    }

    // 손수건과 충돌이 감지되면 즉시 ScenarioManager.cs에 전달
    private IEnumerator HandleHandkerGrab()
    {
        ScenarioManager.Instance.HandkerGrabbed();
        yield break;
    }

    // Scene에서 인식 범위 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
