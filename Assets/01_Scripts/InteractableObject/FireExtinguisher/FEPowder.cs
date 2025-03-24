using UnityEngine;

public class FEPowder : MonoBehaviour
{
	private void Awake()
	{
		Destroy(this.gameObject, 3f);
	}
}