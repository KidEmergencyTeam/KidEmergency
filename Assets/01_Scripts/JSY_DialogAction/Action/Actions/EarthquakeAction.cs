using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EarthquakeAction : MonoBehaviour, IActionEffect
{
    public Transform cameraParent; // 카메라의 부모 객체 (VR 카메라 부모)
    public float shakeDuration = 4f;    // 흔들림 지속 시간
    public float shakeAmount = 0.05f;    // 흔들림 강도
    public float decreaseFactor = 1.0f; // 흔들림 감소 속도
    public float earthquakeForce = 2f;  // 지진의 힘
    public float objectShakeDuration = 4f; // 물체 흔들림 지속 시간
    public Light lightObject;

    private Rigidbody[] _rbObjects;  // 흔들릴 물체들
    private bool _isComplete = false;
    
    public bool IsActionComplete => _isComplete;
    
    private void Start()
    {
        _rbObjects = FindObjectsOfType<Rigidbody>();  
    }

    public void StartAction()
    {
        print("액션 시작");
        _isComplete = false;
        StartCoroutine(EarthquakeRoutine());
    }

    private IEnumerator EarthquakeRoutine()
    {
        while (shakeDuration > 0 || objectShakeDuration > 0)
        {
            EarthquakeStart();
            yield return null;
        }
        
        _isComplete = true;
    }

    private void EarthquakeStart()
    {
        if (shakeDuration > 0 && cameraParent != null)
        {
            cameraParent.localPosition = Random.insideUnitSphere * shakeAmount; 
            shakeDuration -= Time.deltaTime * decreaseFactor;
            SetLight();
        }
        else
        {
            Vector3 originPos = cameraParent.localPosition;
            cameraParent.localPosition = Vector3.zero;
            Vector3 newPos = cameraParent.localPosition;
            Mathf.Lerp(originPos.x, newPos.x, 1);
            lightObject.intensity = 0.6f;
        }

        if (objectShakeDuration > 0)
        {
            foreach (Rigidbody rb in _rbObjects)
            {
                Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, .5f));
                rb.AddForce(randomDirection * (earthquakeForce * 0.1f), ForceMode.Impulse);
            }

            objectShakeDuration -= Time.deltaTime; 
        }
    }

    private void SetLight()
    {
        for (int i = 0; i < 4; i++)
        {
            lightObject.intensity = Random.Range(0, 0.1f);
            lightObject.intensity = Random.Range(0.8f, 1f);
        }
    }
}

