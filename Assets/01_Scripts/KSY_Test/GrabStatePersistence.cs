using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 전환 시에도 손수건 유지해 주는 스크립트
public class GrabStatePersistence : DisableableSingleton<GrabStatePersistence>
{
    [Header("손수건")]
    public Grabbable grabbableComponent;

    // 이벤트 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드 후, Hand 태그를 가진 새로운 손 객체에서 Grabber를 갱신
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (grabbableComponent != null)
        {
            // Hand 태그를 가진 새 손(Grabber) 오브젝트를 찾기
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

            // grabbed object의 currentGrabber를 갱신하기
            grabbableComponent.currentGrabber = newGrabber;
        }
    }

    // 이벤트 제거
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
