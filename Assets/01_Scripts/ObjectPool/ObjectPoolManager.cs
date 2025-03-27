using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	static public ObjectPoolManager Instance { get; set; }

	public Dictionary<string, Queue<GameObject>> objectInPoolDic = new Dictionary<string, Queue<GameObject>>();
	public Dictionary<string, GameObject> poolDic = new Dictionary<string, GameObject>();
	public int poolSize = 10;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			DestroyImmediate(this);
		}
	}

	public GameObject Spawn(GameObject go)
	{
		if (!objectInPoolDic.ContainsKey(go.name + "(Clone)"))
		{
			objectInPoolDic.Add(go.name + "(Clone)", new Queue<GameObject>());
			GameObject obj = new GameObject($"{go.name}(Clone)Pool");
			for (int i = 0; i < poolSize; i++)
			{
				GameObject prefab = Instantiate(go, obj.transform);
				prefab.SetActive(false);
				objectInPoolDic[go.name + "(Clone)"].Enqueue(prefab);
			}

			poolDic.Add(obj.name, obj);
		}

		if (objectInPoolDic[go.name + "(Clone)"].Count == 0)
		{
			GameObject prefab = Instantiate(go, poolDic[$"{go.name}(Clone)Pool"].transform);
			prefab.SetActive(false);
			objectInPoolDic[go.name + "(Clone)"].Enqueue(prefab);
		}

		GameObject unit = objectInPoolDic[go.name + "(Clone)"].Dequeue();
		unit.SetActive(true);
		return unit;
	}

	public GameObject Spawn(GameObject go, Vector3 position, Quaternion rotation)
	{
		GameObject unit = Spawn(go);
		unit.transform.position = position;
		unit.transform.rotation = rotation;
		return unit;
	}

	public GameObject Spawn(GameObject go, Transform parent)
	{
		GameObject unit = Spawn(go);
		unit.transform.SetParent(parent);
		return unit;
	}

	public void ReturnToPool(GameObject go)
	{
		go.SetActive(false);
		ResetTransform(go);
		go.transform.SetParent(poolDic[$"{go.name}Pool"].transform);
		objectInPoolDic[go.name].Enqueue(go);
	}

	public void ReturnToPool(GameObject go, float delay)
	{
		StartCoroutine(StartDelay(go, delay));
	}

	IEnumerator StartDelay(GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		ReturnToPool(go);
	}

	private void ResetTransform(GameObject go)
	{
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}
}