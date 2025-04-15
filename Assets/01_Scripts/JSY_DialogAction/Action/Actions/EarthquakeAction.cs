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
    public Light[] lightObjects;
    public Light mainLight; // 영향을 제일 많이 주는 메인 조명
    public RobotController seti;

    [Header("오디오 클립")] public AudioClip eqAudio;
    
    private Rigidbody[] _rbObjects;  // 흔들릴 물체들
    private bool _isComplete = false;
    
    public bool IsActionComplete => _isComplete;
    
    private void Start()
    {
        _rbObjects = FindObjectsOfType<Rigidbody>(); 
    }

    public void StartAction()
    {
        _isComplete = false;
        StartCoroutine(EarthquakeRoutine());
    }

    private IEnumerator EarthquakeRoutine()
    {
        while (shakeDuration > 0)
        {
            seti.SetLookingFor();
            EarthquakeStart();
            yield return null;
        }
        
        seti.SetAnimaiton("LookingFor");
        _isComplete = true;
    }

    private void EarthquakeStart()
    {
        if (shakeDuration > 0 && cameraParent != null)
        {
            if (ActionManager.Instance.actionAudio.clip == null)
            {
                ActionManager.Instance.actionAudio.clip = eqAudio;
                ActionManager.Instance.actionAudio.Play();
            }
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
            mainLight.intensity = 0.6f;
            for (int i = 0; i < lightObjects.Length; i++)
            {
                lightObjects[i].intensity = Random.Range(0.2f, 0.7f);
            }
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
            mainLight.intensity = Random.Range(0.1f, 0.5f);
            mainLight.intensity = Random.Range(0.7f, 1f);
            for (int j = 0; j < lightObjects.Length; j++)
            {
                lightObjects[j].intensity = Random.Range(0.1f, 0.5f);
                lightObjects[j].intensity = Random.Range(0.7f, 1f);
            }
        }
    }
}

