using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 전환 시에도 손수건 유지해 주는 스크립트
public class GrabStatePersistence : DisableableSingleton<GrabStatePersistence>
{
    // 손수건 
    [Header("손수건")]
    public Grabbable grabbableComponent;

    // grabbed object를 저장하고, 루트 오브젝트에 DontDestroyOnLoad를 적용합니다.
    public void SaveGrabState(Grabbable grabbed)
    {
        if (grabbed == null)
        {
            Debug.LogWarning("Grabbable이 할당되지 않았습니다.");
            return;
        }

        grabbableComponent = grabbed;
    }

    // 이벤트 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드 후, Hand 태그를 가진 새로운 손 객체에서 Grabber를 갱신합니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (grabbableComponent != null)
        {
            // Hand 태그를 가진 새 손(Grabber) 오브젝트를 찾습니다.
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

            // grabbed object의 currentGrabber를 갱신합니다.
            grabbableComponent.currentGrabber = newGrabber;
        }
    }

    // 이벤트 제거
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
