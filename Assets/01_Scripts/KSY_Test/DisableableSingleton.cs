using UnityEngine;

// true 값을 받아 싱글톤 객체 삭제
public class DisableableSingleton<T> : SingletonManager<T> where T : MonoBehaviour
{
    // 값 만 저장
    // false -> 정상 진행
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
                _disableSingleton = false;
            }
            else
            {
                _disableSingleton = value;
            }
        }
    }
}
