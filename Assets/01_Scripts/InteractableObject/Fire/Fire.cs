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

		if (fireHp > 20)
		{
			transform.localScale = originalScale * fireHp / startFireHp;
		}
		else if (FEScene.Instance.currentState != FEScene.Instance.states[FEStateType.FEDialog])
		{
			JSWDialogManager.Instance.delay = 1f;
			FEScene.Instance.currentDialogIndex = 15;
			FEScene.Instance.ChangeState(FEStateType.FEDialog);
			Destroy(gameObject);
		}
	}
}