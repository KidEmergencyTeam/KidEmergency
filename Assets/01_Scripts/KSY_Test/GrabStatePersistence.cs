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

    // 씬 로드 후, Hand 태그를 가진 새로운 Left Controller 오브젝트에 추가된 Grabber 컴포넌트를 찾아
    // 손수건 오브젝트에 추가된 Grabbable 컴포넌트에서 currentGrabber를 갱신
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (grabbableComponent != null)
        {
            // Hand 태그를 가진 새로운 Left Controller 오브젝트 찾기
            GameObject handObj = GameObject.FindGameObjectWithTag("Hand");
            if (handObj == null)
            {
                Debug.LogWarning("Hand 태그의 Left Controller 오브젝트를 찾을 수 없습니다.");
                return;
            }

            // Hand 태그를 가진 새로운 Left Controller 오브젝트에서
            // Grabber 컴포넌트 가져오기
            Grabber newGrabber = handObj.GetComponent<Grabber>();
            if (newGrabber == null)
            {
                Debug.LogWarning("Hand 태그의 Left Controller 오브젝트에서 Grabber 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            // 손수건: 씬마다 손 위치와 방향 갱신
            grabbableComponent.currentGrabber = newGrabber;

            // 손: 손수건을 잡았을때 갱신된 위치와 방향을 따라 이동
            if (!grabbableComponent.isGrabbable)
            {
                newGrabber.OnGrab(grabbableComponent);
            }
        }
    }

    // 이벤트 제거
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        grabbableComponent.currentGrabber.OnRelease();
    }
}
