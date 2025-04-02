using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FEHose : MonoBehaviour
{
	public Grabbable grabbable;
	public ChainIKConstraint hoseIKChain;

	private void Start()
	{
		grabbable = GetComponent<Grabbable>();
	}
}