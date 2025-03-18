using Fusion;
using UnityEngine;

public class NetworkHeadset : MonoBehaviour
{
	public NetworkTransform networkTransform;

	private void Awake()
	{
		networkTransform = GetComponent<NetworkTransform>();
	}
}
