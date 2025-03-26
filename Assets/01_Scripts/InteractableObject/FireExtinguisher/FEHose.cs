using UnityEngine;

public class FEHose : MonoBehaviour
{
	public Grabbable grabbable;

	private void Start()
	{
		grabbable = GetComponent<Grabbable>();
	}
}