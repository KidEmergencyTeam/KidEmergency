using UnityEngine;

public class Fire : MonoBehaviour
{
	public float startFireHp = 100f;
	public float fireHp;
	public Vector3 originalScale;
	public bool is30 = false;

	private void Awake()
	{
		originalScale = transform.localScale;
		fireHp = startFireHp;
	}

	public void TakeDamage(float damage)
	{
		fireHp -= damage;

		if (fireHp < 10)
		{
			JSWDialogManager.Instance.delay = 1f;
			FEScene.Instance.currentDialogIndex = 14;
			FEScene.Instance.ChangeState(FEStateType.FEDialog);
			Destroy(gameObject);
		}

		transform.localScale = originalScale * fireHp / startFireHp;
	}
}