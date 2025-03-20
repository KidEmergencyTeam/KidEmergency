using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this);
		}
	}
}