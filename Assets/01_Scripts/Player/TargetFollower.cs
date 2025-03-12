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
		public bool followPosition = true;
		public bool followRotation = true;
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

			if (followTarget.followPosition)
				followTarget.follower.position = followTarget.target.position;

			if (followTarget.followRotation)
				followTarget.follower.rotation = followTarget.target.rotation;
		}
	}
}