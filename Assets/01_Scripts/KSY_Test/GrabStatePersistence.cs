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

            // 손수건 오브젝트에 추가된
            // Grabbable 컴포넌트에서 currentGrabber를 갱신하면
            // Grabbable 컴포넌트에서 손수건의 위치와 방향을
            // Grabber 컴포넌트가 추가된 오브젝트의 위치와 방향으로 처리
            grabbableComponent.currentGrabber = newGrabber;
        }
    }

    // 이벤트 제거
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
