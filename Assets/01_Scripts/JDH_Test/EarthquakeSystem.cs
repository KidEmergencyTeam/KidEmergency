using System.Collections;
using UnityEngine;

public class EarthquakeSystem : MonoBehaviour
{
    public Transform cameraParent; // ī�޶��� �θ� ��ü (VR ī�޶� �θ�)
    public float shakeIntensity = 0.1f; // ī�޶� ��鸲 ����
    public float shakeFadeSpeed = 1.0f; // ��鸲 ���� �ӵ�
    public float objectShakeForce = 2f; // ��ü ��鸲 ��
    public Light[] lightObjects;
    public Light mainLight; // ���� ���� (������ ȿ��)

    private Rigidbody[] _rbObjects; // ��鸱 ��ü��
    private Vector3 _originalCameraPos;
    private bool _isShaking = false;
    public bool _endEarthquake = false; // `false`�� �� ������ �ݺ�

    private void Start()
    {
        _rbObjects = FindObjectsOfType<Rigidbody>();
        if (cameraParent != null)
            _originalCameraPos = cameraParent.localPosition;
    }

    public void StartEarthquake()
    {
        if (_isShaking) return; // �ߺ� ���� ����

        Debug.Log("���� ����!");
        _isShaking = true;
        _endEarthquake = false; // ���� ���� �� false ����
        StartCoroutine(EarthquakeRoutine());
    }

    private IEnumerator EarthquakeRoutine()
    {
        while (!_endEarthquake) // `_endEarthquake`�� false�� ���� ��� �ݺ�
        {
            ApplyCameraShake();
            ApplyObjectShake();
            ApplyLightFlicker();
            yield return null;
        }

        StopEarthquake(); // ���� ���� ó��
    }

    public void StopEarthquake()
    {
        Debug.Log("���� ����!");
        _isShaking = false;
        _endEarthquake = true; // ���� ����

        // ��鸲�� ������ ���� ���·� ����
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
