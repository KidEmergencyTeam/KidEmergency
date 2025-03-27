using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableableSingleton<T> : SingletonManager<T> where T : MonoBehaviour
{
    // 값 만 저장
    private bool _disableSingleton = false;

    // 제네릭 타입.Instance.disableSingleton = true; -> 이런식으로 호출
    // false에서 true로 전환되면 즉시 오브젝트를 파괴
    public bool disableSingleton
    {
        get { return _disableSingleton; }
        set
        {
            // 값이 변경되어 true가 되면 즉시 오브젝트를 파괴
            if (!_disableSingleton && value)
            {
                _disableSingleton = value;
                Debug.Log($"{typeof(T).Name}: disableSingleton 값이 true로 변경되어 오브젝트를 파괴");
                Destroy(gameObject);
            }
            else
            {
                _disableSingleton = value;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 전환 로드가 완료되면 플래그를 초기화
    private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        Debug.Log("씬 전환 로드 완료 - GlobalDisableAllSingletons 초기화");
        disableSingleton = false;
    }
}
