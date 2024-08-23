using System.Collections;
using UnityEngine;
using SnowHorse.Utils;

public enum DoorState { Locked, Closed, Open };
public class Behaviour_DoorNew : MonoBehaviour, IBehaviour
{
	[SerializeField] private GameObject _key;

	[Header("Door Properties")]
	[SerializeField] private Transform _doorPivot;
	[SerializeField] private Transform _openRotation;
	[SerializeField] private Transform _closedRotation;
	[SerializeField] private Collider[] _colliders;
	[SerializeField] private float _movementDurationInSeconds;

	[Header("Door Audio")]
	[SerializeField] private AudioClip _isLockedClip;
	[SerializeField] private AudioClip _isUnlockedClip;
	[SerializeField] private AudioClip _isOpeningClip;
	[SerializeField] private AudioClip _isClosingClip;
	[SerializeField] private AudioSource _audioSource;
	[SerializeField] private float _volume;

	[Header("Use in case door illumination is needed")]
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Material _emmisiveMat;
	[SerializeField] private Material _opaqueMat;
	[SerializeField] private GameObject _lights;


	public DoorState CurrentDoorState;
	private Vector3 _initialRotation;
	private bool _changeDoorState;
	private float _currentLerpPercentage;

	public float rotationSpeed = 5f;
	private bool isGrabbed = false;
	private Vector3 grabPoint;
	private float previousAngle = 0f;

	private void Start()
	{
		_initialRotation = _doorPivot.transform.rotation.eulerAngles;
	}

	public void Behaviour(bool isInteracting, bool isInspecting)
	{
		if(isInteracting)
		{
			grabPoint = PlayerController.Instance.InteractHitPoint;
			isGrabbed = true;
			PlayerController.Instance.FreezePlayerMovement = true;
			// Store the current angle for next frame
			previousAngle = _doorPivot.transform.localEulerAngles.y;

		}

		//switch (CurrentDoorState)
		//{
		//	case DoorState.Locked:
		//		{
		//			PlayerInventory inventory = PlayerController.Instance.Inventory;

		//			if (_key != null && inventory.SelectedItem() == _key)
		//			{
		//				inventory.Remove(_key);
		//				Destroy(_key);

		//				_audioSource.PlayOneShot(_isUnlockedClip, _volume * GameController.Instance.GlobalVolume); //Unlock sound

		//				CurrentDoorState = DoorState.Closed;
		//				_changeDoorState = true;
		//				//StartCoroutine(ManageDuration(_movementDurationInSeconds));
		//			}
		//			else
		//			{
		//				_audioSource.PlayOneShot(_isLockedClip, _volume * GameController.Instance.GlobalVolume); //Locked sound
		//			}
		//			break;
		//		}
		//	default:
		//		{
		//			_initialRotation = _doorPivot.localRotation.eulerAngles;
		//			_changeDoorState = true;
		//			//StartCoroutine(ManageDuration(_movementDurationInSeconds));
		//			break;
		//		}
		//}
	}

	private void RotateDoor()
	{
		var currentInteractable = PlayerController.Instance.InteractableInSight;
		var hitPoint = PlayerController.Instance.InteractHitPoint;
		if (!currentInteractable || currentInteractable.gameObject != gameObject && hitPoint == Vector3.zero) return;

		// Calculate the rotation angle based on the hitpoint position
		Vector3 doorCenter = _doorPivot.transform.position;
		Vector3 doorUp = _doorPivot.transform.up;
		Vector3 hitDirection = (hitPoint - doorCenter).normalized;
		float targetAngle = Mathf.Atan2(hitDirection.x, hitDirection.z) * Mathf.Rad2Deg;

		// Rotate the door
		_doorPivot.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(previousAngle, targetAngle, Time.deltaTime * 10f), 0);

		// Store the current angle for next frame
		previousAngle = _doorPivot.transform.localEulerAngles.y;
	}

	void Update()
	{


		if (isGrabbed)
		{
			RotateDoor();

			if(Input.GetMouseButtonUp(0))
			{
				isGrabbed = false;
				PlayerController.Instance.FreezePlayerMovement = false;
			}
		}

		//if (_renderer != null)
		//{
		//	ManageLights();
		//}

		//ManageRotation();
	}

	//private void ManageRotation()
	//{
	//	if (_changeDoorState)
	//	{
	//		float percentage = Interpolation.Smoother(_movementDurationInSeconds, ref _currentLerpPercentage);

	//		switch (CurrentDoorState)
	//		{
	//			case DoorState.Open:
	//				{
	//					_doorPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(_initialRotation), _closedRotation.localRotation, percentage);
	//					break;
	//				}
	//			case DoorState.Closed:
	//				{
	//					_doorPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(_initialRotation), _openRotation.localRotation, percentage);
	//					break;
	//				}
	//		}
	//	}
	//}

	//private IEnumerator ManageDuration(float duration)
	//{
	//	switch (CurrentDoorState)
	//	{
	//		case DoorState.Open:
	//			{
	//				_audioSource.PlayOneShot(_isClosingClip, _volume * GameController.Instance.GlobalVolume);
	//				break;
	//			}
	//		case DoorState.Closed:
	//			{
	//				_audioSource.PlayOneShot(_isOpeningClip, _volume * GameController.Instance.GlobalVolume);
	//				break;
	//			}
	//	}

	//	for (int i = 0; i < _colliders.Length; i++)
	//	{
	//		_colliders[i].enabled = false;
	//	}

	//	yield return new WaitForSecondsRealtime(duration);

	//	switch (CurrentDoorState)
	//	{
	//		case DoorState.Open:
	//			{
	//				CurrentDoorState = DoorState.Closed;
	//				break;
	//			}
	//		case DoorState.Closed:
	//			{
	//				CurrentDoorState = DoorState.Open;
	//				break;
	//			}
	//	}

	//	_changeDoorState = false;

	//	for (int i = 0; i < _colliders.Length; i++)
	//	{
	//		_colliders[i].enabled = true;
	//	}

	//	_currentLerpPercentage = 0; //reset lerp percentage for next time
	//}

	//private void ManageLights()
	//{
	//	if (CurrentDoorState == DoorState.Locked)
	//	{
	//		GameObject playerSelectedItem = PlayerController.Instance.Inventory.SelectedItem();

	//		if (playerSelectedItem != null && playerSelectedItem == _key)
	//		{
	//			_lights.SetActive(true);
	//			_renderer.material = _emmisiveMat;
	//		}
	//		else
	//		{
	//			_lights.SetActive(false);
	//			_renderer.material = _opaqueMat;
	//		}
	//	}
	//	else
	//	{
	//		_lights.SetActive(true);
	//		_renderer.material = _opaqueMat;
	//	}
	//}

	public bool IsInteractable()
	{
		return true;
	}

	public bool IsInspectable()
	{
		return false;
	}
}