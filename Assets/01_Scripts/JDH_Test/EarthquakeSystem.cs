using System.Collections;
using UnityEngine;

public class EarthquakeSystem : MonoBehaviour
{
    public Transform cameraParent; // 카메라의 부모 객체 (VR 카메라 부모)
    public float shakeIntensity = 0.1f; // 카메라 흔들림 강도
    public float shakeFadeSpeed = 1.0f; // 흔들림 감소 속도
    public float objectShakeForce = 2f; // 물체 흔들림 힘
    public Light[] lightObjects;
    public Light mainLight; // 메인 조명 (깜빡임 효과)

    private Rigidbody[] _rbObjects; // 흔들릴 물체들
    private Vector3 _originalCameraPos;
    private bool _isShaking = false;
    public bool _endEarthquake = false; // `false`가 될 때까지 반복

    private void Start()
    {
        _rbObjects = FindObjectsOfType<Rigidbody>();
        if (cameraParent != null)
            _originalCameraPos = cameraParent.localPosition;
    }

    public void StartEarthquake()
    {
        if (_isShaking) return; // 중복 실행 방지

        Debug.Log("지진 시작!");
        _isShaking = true;
        _endEarthquake = false; // 지진 시작 시 false 설정
        StartCoroutine(EarthquakeRoutine());
    }

    private IEnumerator EarthquakeRoutine()
    {
        while (!_endEarthquake) // `_endEarthquake`가 false인 동안 계속 반복
        {
            ApplyCameraShake();
            ApplyObjectShake();
            ApplyLightFlicker();
            yield return null;
        }

        StopEarthquake(); // 지진 종료 처리
    }

    public void StopEarthquake()
    {
        Debug.Log("지진 종료!");
        _isShaking = false;
        _endEarthquake = true; // 지진 종료

        // 흔들림이 끝나면 원래 상태로 복구
        if (cameraParent != null)
            cameraParent.localPosition = _originalCameraPos;

        if (mainLight != null)
            mainLight.intensity = 1f;
    }

    private void ApplyCameraShake()
    {
        if (cameraParent != null)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            cameraParent.localPosition = Vector3.Lerp(cameraParent.localPosition, _originalCameraPos + shakeOffset, Time.deltaTime * 10);
        }
    }

    private void ApplyObjectShake()
    {
        foreach (Rigidbody rb in _rbObjects)
        {
            Vector3 randomForce = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.1f, 0.5f),
                Random.Range(-1f, 1f)
            ) * objectShakeForce;

            rb.AddForce(randomForce, ForceMode.Impulse);
        }
    }

    private void ApplyLightFlicker()
    {
        if (mainLight != null)
            mainLight.intensity = Random.Range(0.5f, 1.2f);

        foreach (Light light in lightObjects)
        {
            if (light != null)
                light.intensity = Random.Range(0.3f, 1.0f);
        }
    }
}
