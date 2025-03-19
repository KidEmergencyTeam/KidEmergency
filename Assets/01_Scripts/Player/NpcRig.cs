using UnityEngine;

public class NpcRig : MonoBehaviour
{
	private enum State
	{
		None,
		Hold,
		Bow
	}

	[System.Serializable]
	private class PoseData
	{
		public Vector3 playerPos;
		public Vector3 playerRot;
		public Vector3 leftHandPos;
		public Vector3 leftHandRot;
		public Vector3 rightHandPos;
		public Vector3 rightHandRot;
	}

	[SerializeField] private State _state = State.None;

	public Transform player;
	public Transform leftHandTarget;
	public Transform rightHandTarget;

	[Header("Pose Data")] [SerializeField] private PoseData noneState;
	[SerializeField] private PoseData holdState;
	[SerializeField] private PoseData bowState;

	private void Update()
	{
		switch (_state)
		{
			case State.None:
				SetState(noneState);
				break;
			case State.Hold:
				SetState(holdState);
				break;
			case State.Bow:
				SetState(bowState);
				break;
		}
	}

	private void SetState(PoseData pose)
	{
		player.localPosition = pose.playerPos;
		player.rotation = Quaternion.Euler(pose.playerRot);

		leftHandTarget.localPosition = pose.leftHandPos;
		leftHandTarget.rotation = Quaternion.Euler(pose.leftHandRot);

		rightHandTarget.localPosition = pose.rightHandPos;
		rightHandTarget.rotation = Quaternion.Euler(pose.rightHandRot);
	}
}