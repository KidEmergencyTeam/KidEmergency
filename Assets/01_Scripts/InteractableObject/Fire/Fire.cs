using System;
using UnityEngine;

public class Fire : MonoBehaviour
{
	public float fireHp = 100f;
	public Vector3 originalScale;

	private void Awake()
	{
		originalScale = transform.localScale;
	}

	public void TakeDamage(float damage)
	{
		fireHp -= damage;
		if (fireHp < 10) Destroy(gameObject);
		transform.localScale = originalScale * fireHp / 100f;
	}
}