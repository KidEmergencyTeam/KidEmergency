using UnityEngine;

[DefaultExecutionOrder(Grabbable.ExecutionOrder)]
[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
	public const int ExecutionOrder = 100;
	public bool isGrabbable = true; //그랩 가능한 상태, 스토리 진행에 따라 조절
	public Vector3 grabPosOffset;
	public Vector3 grabRotOffset;

	public bool
		isSameMoveAndGrabbable =
			true; //그랩이 가능하고 아웃라인이 나오는 오브젝트와 실제 움직이는 오브젝트가 다른 경우 false

	public GameObject realMovingObject; //실제 움직이는 오브젝트
	public bool isLeft = true; //상호작용 가능한 손의 위치
	public bool isMoving = true; //손을 따라 움직이는 Object면 true, Object에 손이 박히면 false

	[HideInInspector] public Rigidbody rb;
	public Grabber currentGrabber;

	//그랩 가능한 오브젝트 Ctrl+D 하고 메쉬 렌더러와 메쉬 필터를 제외한 컴포넌트 제거
	//스케일은 1.01
	////머티리얼은 02_Textures > Materials에 있는 GrabbableHighlight 넣어주기
	public GameObject highlight;

	protected Highlighter highlighter;
	protected bool isTrigger = false;

	public bool IsGrabbed => currentGrabber;

	protected virtual void Start()
	{
		if (!this.TryGetComponent<Collider>(out Collider c))
		{
			Debug.LogWarning(this.gameObject.name + " 콜라이더 없음");
		}

		rb = GetComponent<Rigidbody>();
		if (isSameMoveAndGrabbable) realMovingObject = this.gameObject;
		highlighter = highlight.GetComponent<Highlighter>();
		highlight.SetActive(false);
	}

	private void OnTriggerStay(Collider other)
	{
		if (IsGrabbed) return;
		if (!other.TryGetComponent<Grabber>(out Grabber grabber)) return;
		if (isGrabbable && grabber.isLeft == isLeft)
		{
			isTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		print(other.name);
		if (IsGrabbed) return;
		if (!other.TryGetComponent<Grabber>(out Grabber grabber)) return;
		if (isGrabbable && grabber.isLeft == isLeft)
		{
			isTrigger = false;
		}
	}

	protected virtual void Update()
	{
		if (!isGrabbable)
		{
			highlight.SetActive(false);

			if (!IsGrabbed) return;

			if (isMoving)
			{
				realMovingObject.transform.position =
					currentGrabber.transform.position + grabPosOffset;
				realMovingObject.transform.rotation =
					currentGrabber.transform.rotation * Quaternion.Euler(grabRotOffset);
			}
		}
		else
		{
			highlight.SetActive(true);
			if (isTrigger)
			{
				highlighter.SetColor(Color.green);
				highlighter.isBlinking = false;
			}
			else
			{
				highlighter.SetColor(Color.yellow);
				highlighter.isBlinking = true;
			}
		}
	}
}