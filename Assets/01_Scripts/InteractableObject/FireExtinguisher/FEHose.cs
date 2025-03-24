using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class FEHose : MonoBehaviour
{
	public float extinguishingSpeed = 10f;
	public float currentExtinguishingSpeed;
	public Transform muzzle;
	public Grabbable grabbable;
	public float maxDistance = 10f;
	public FirePosition currentFire = FirePosition.None;

	private void Awake()
	{
		currentExtinguishingSpeed = extinguishingSpeed;
	}

	private void Start()
	{
		grabbable = GetComponent<Grabbable>();
	}

	private void Update()
	{
		Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, maxDistance);
		Gizmos.DrawRay(muzzle.position, muzzle.forward * maxDistance);
		if (!hit.transform.TryGetComponent<FireDetectCollider>(
			    out FireDetectCollider fire)) return;
		if (currentFire != FirePosition.None)
		{
			currentExtinguishingSpeed = extinguishingSpeed;
		}

		currentFire = fire.firePosition;
	}
}