using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
	[Serializable]
	public class FollowTarget
	{
		public Transform target; // 따라갈 대상
		public Transform follower; // 따라가는 오브젝트
		[Header("Position")] public bool isFollowPos = true;
		public Vector3 posOffset;
		[Header("Rotation")] public bool isFollowRot = true;
		public Vector3 rotOffset;

		public void Follow()
		{
			if (isFollowPos)
			{
				follower.position = target.TransformPoint(posOffset);
			}

			if (isFollowRot)
			{
				follower.rotation = target.rotation * Quaternion.Euler(rotOffset);
			}
		}
	}

	public List<FollowTarget> followTargets = new List<FollowTarget>();

	private void LateUpdate()
	{
		foreach (var followTarget in followTargets)
		{
			if (followTarget.target == null || followTarget.follower == null)
			{
				Debug.LogWarning("Target or Follower is null.");
				continue;
			}

			followTarget.Follow();
		}
	}
}