using UnityEngine;

public class NpcRig : MonoBehaviour
{
	public enum State
	{
		None,
		Hold,
		Bow,
		DownDesk,
		HoldDesk,
		HoldBag
	}

	[System.Serializable]
	private class PoseData
	{
		public Vector3 bodyPos;
		public Vector3 bodyRot;
		public Vector3 leftHandPos;
		public Vector3 leftHandRot;
		public Vector3 rightHandPos;
		public Vector3 rightHandRot;

		public bool isHandkerchiefActive;
		public bool isBagActive;
	}

	public State state = State.None;

	public GameObject handkerchief;
	public GameObject bag;

	public Transform body;
	public Transform leftHandTarget;
	public Transform rightHandTarget;

	[Header("Pose Data")] [SerializeField] private PoseData noneState;
	[SerializeField] private PoseData holdState;
	[SerializeField] private PoseData bowState;
	[SerializeField] private PoseData downDeskState;
	[SerializeField] private PoseData holdDeskState;
	[SerializeField] private PoseData holdBagState;

	private void Update()
	{
		switch (state)
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
			case State.DownDesk:
				SetState(downDeskState);
				break;
			case State.HoldDesk:
				SetState(holdDeskState);
				break;
			case State.HoldBag:
				SetState(holdBagState);
				break;
		}
	}

	private void SetState(PoseData pose)
	{
		body.localPosition = pose.bodyPos;
		body.localRotation = Quaternion.Euler(pose.bodyRot);

		leftHandTarget.localPosition = pose.leftHandPos;
		leftHandTarget.localRotation = Quaternion.Euler(pose.leftHandRot);

		rightHandTarget.localPosition = pose.rightHandPos;
		rightHandTarget.localRotation = Quaternion.Euler(pose.rightHandRot);

		handkerchief.gameObject.SetActive(pose.isHandkerchiefActive);
		bag.gameObject.SetActive(pose.isBagActive);
	}
}