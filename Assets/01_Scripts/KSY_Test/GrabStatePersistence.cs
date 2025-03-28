using UnityEngine;
using UnityEngine.SceneManagement;

public class GrabStatePersistence : DisableableSingleton<GrabStatePersistence>
{
    // 지속되어야 할 Grabbable 컴포넌트 (잡힌 오브젝트)
    [SerializeField] private Grabbable grabbableComponent;

    // 저장된 오프셋 및 잡힌 손의 정보
    public Vector3 savedGrabPosOffset;
    public Vector3 savedGrabRotOffset;
    public bool savedIsLeft;

    // 잡힌 상태일 때, 해당 Grabbable의 오프셋과 손 정보를 저장합니다.
    public void SaveGrabState(Grabbable grabbed)
    {
        if (grabbed == null)
        {
            Debug.LogWarning("Grabbable이 할당되지 않았습니다.");
            return;
        }

        savedGrabPosOffset = grabbed.grabPosOffset;
        savedGrabRotOffset = grabbed.grabRotOffset;
        savedIsLeft = grabbed.isLeft;
        grabbableComponent = grabbed;

        // 잡힌 상태라면 grabbed object가 씬 전환 시 파괴되지 않도록 설정합니다.
        DontDestroyOnLoad(grabbableComponent.gameObject);
    }

    // 씬 로드 후, Hand 태그를 가진 새 손을 찾아 grabbed object에 적용합니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (grabbableComponent != null)
        {
            // Hand 태그로 새 손(Grabber) 오브젝트를 찾습니다.
            GameObject handObj = GameObject.FindGameObjectWithTag("Hand");
            if (handObj == null)
            {
                Debug.LogWarning("새로운 Grabber(Hand 태그)를 찾을 수 없습니다.");
                return;
            }

            Grabber newGrabber = handObj.GetComponent<Grabber>();
            if (newGrabber == null)
            {
                Debug.LogWarning("Hand 태그의 객체에서 Grabber 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            // 저장된 isLeft 값과 일치하는지 확인 (필요한 경우)
            if (newGrabber.isLeft != savedIsLeft)
            {
                Debug.LogWarning("새로운 Grabber의 isLeft 값이 저장된 값과 다릅니다.");
            }

            // grabbed object의 currentGrabber를 새 손으로 갱신합니다.
            grabbableComponent.currentGrabber = newGrabber;

            // 저장된 오프셋을 적용하여 grabbed object의 위치와 회전을 보정합니다.
            grabbableComponent.transform.position = newGrabber.transform.position + savedGrabPosOffset;
            grabbableComponent.transform.rotation = newGrabber.transform.rotation * Quaternion.Euler(savedGrabRotOffset);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
