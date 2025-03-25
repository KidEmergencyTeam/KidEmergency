using UnityEngine;

public class FEPowder : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = gameObject.GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (ps.isStopped) ObjectPoolManager.Instance.ReturnToPool(gameObject);
	}
}