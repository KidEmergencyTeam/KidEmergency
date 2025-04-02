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

	public GameObject highlight; //깜빡이는 오브젝트

	protected Material _highlightMaterial;
	protected bool _alphaUp = false;
	protected float _currentAlpha;
	protected bool _isTrigger = false;

	public bool IsGrabbed => currentGrabber;

	protected virtual void Start()
	{
		if (!this.TryGetComponent<Collider>(out Collider c))
		{
			Debug.LogWarning(this.gameObject.name + " 콜라이더 없음");
		}

		rb = GetComponent<Rigidbody>();
		if (isSameMoveAndGrabbable) realMovingObject = this.gameObject;
		_highlightMaterial = highlight.GetComponent<Renderer>().material;
		highlight.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		print(other.name);
		if (IsGrabbed) return;
		if (!other.TryGetComponent<Grabber>(out Grabber grabber)) return;
		if (isGrabbable && grabber.isLeft == isLeft)
		{
			_isTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		print(other.name);
		if (IsGrabbed) return;
		if (!other.TryGetComponent<Grabber>(out Grabber grabber)) return;
		if (isGrabbable && grabber.isLeft == isLeft)
		{
			_isTrigger = false;
		}
	}

	protected virtual void Update()
	{
		if (isGrabbable)
		{
			highlight.SetActive(true);
			if (_isTrigger)
			{
				_highlightMaterial.color = new Color(0, 1, 0, 0.7f);
			}
			else
			{
				print("currentAlpha: " + _currentAlpha);
				_highlightMaterial.color = new Color(1, 1, 0, _currentAlpha);
				if (_alphaUp)
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 0.7f, Time.deltaTime * 0.5f);
					if (_currentAlpha >= 0.69f) _alphaUp = false;
				}
				else
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 0f, Time.deltaTime * 0.5f);
					if (_currentAlpha <= 0.01f) _alphaUp = true;
				}
			}
		}
		else
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
	}
}