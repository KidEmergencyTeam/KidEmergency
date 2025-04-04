using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(FEHandle.ExecutionOrder)]
public class FEHandle : MonoBehaviour
{
	private const int ExecutionOrder = Grabbable.ExecutionOrder + 10;

	public float maxExtinguishingDamage = 10f;
	public float minExtinguishingDamage = 2f;
	public float extinguisherDistance = 10f;
	public FirePosition currentFire = FirePosition.None;
	public Transform muzzle;
	public FEHose hose;
	public GameObject powderPrefab;
	public float fireCoolTime = 0.2f;
	public float currentExtinguishingDamage;
	public InputActionProperty fireAction;

	private Fire _fire;
	private float _stopDamage;
	public float damageSpeed;
	private float _fireEndCoolTime = 0f;
	private Grabbable _grabbable;
	private FireDetectCollider _fireDetectCollider;

	private void Awake()
	{
		damageSpeed = (maxExtinguishingDamage - minExtinguishingDamage) / fireCoolTime * Time.deltaTime;
		currentExtinguishingDamage = maxExtinguishingDamage;
	}

	private void Start()
	{
		_fire = FindObjectOfType<Fire>();
		_grabbable = GetComponent<Grabbable>();
		fireAction = _grabbable.currentGrabber.controllerButtonClick;
	}

	private void Update()
	{
		if (!hose.currentGrabber) return;
		RaycastHit[] hits = Physics.RaycastAll(muzzle.position, muzzle.forward, extinguisherDistance);
		if (hits.Length > 0)
		{
			foreach (RaycastHit hit in hits)
			{
				if (hit.transform.TryGetComponent<FireDetectCollider>(out FireDetectCollider fireCollider))
				{
					_fireDetectCollider = fireCollider;
					break;
				}
				else
				{
					currentFire = FirePosition.None;
					_fireDetectCollider = null;
				}
			}
		}

		if (fireAction.action.ReadValue<float>() > 0)
		{
			print("ButtonClick");
			if (Time.time > _fireEndCoolTime)
			{
				print("time > cooltime");
				_fireEndCoolTime = Time.time + fireCoolTime;
				ObjectPoolManager.Instance.Spawn(powderPrefab, muzzle.position, muzzle.rotation);
			}

			if (!_fireDetectCollider) return;
			if (currentFire != _fireDetectCollider.firePosition)
			{
				currentExtinguishingDamage = Mathf.MoveTowards(maxExtinguishingDamage, minExtinguishingDamage,
					damageSpeed * Time.deltaTime);
				currentFire = _fireDetectCollider.firePosition;
			}
			else
			{
				currentExtinguishingDamage = Mathf.MoveTowards(currentExtinguishingDamage, minExtinguishingDamage,
					damageSpeed * Time.deltaTime);
			}

			_fire.TakeDamage(currentExtinguishingDamage * Time.deltaTime);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(muzzle.position, muzzle.forward * extinguisherDistance);
	}
}