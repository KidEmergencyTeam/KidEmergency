using UnityEngine;

public class NpcRig : MonoBehaviour
{
	public enum State
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

	public State state = State.None;

	public GameObject handkerchief;

	public Transform player;
	public Transform leftHandTarget;
	public Transform rightHandTarget;

	[Header("Pose Data")] [SerializeField] private PoseData noneState;
	[SerializeField] private PoseData holdState;
	[SerializeField] private PoseData bowState;

	private void Update()
	{
		switch (state)
		{
			case State.None:
				handkerchief.gameObject.SetActive(false);
				SetState(noneState);
				break;
			case State.Hold:
				handkerchief.gameObject.SetActive(true);
				SetState(holdState);
				break;
			case State.Bow:
				handkerchief.gameObject.SetActive(true);
				SetState(bowState);
				break;
		}
	}

	private void SetState(PoseData pose)
	{
		player.localPosition = pose.playerPos;
		player.localRotation = Quaternion.Euler(pose.playerRot);

		leftHandTarget.localPosition = pose.leftHandPos;
		leftHandTarget.localRotation = Quaternion.Euler(pose.leftHandRot);

		rightHandTarget.localPosition = pose.rightHandPos;
		rightHandTarget.localRotation = Quaternion.Euler(pose.rightHandRot);
	}
}