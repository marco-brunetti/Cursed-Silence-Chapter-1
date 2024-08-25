using UnityEngine;

public enum DoorState { Locked, Closed, Open };

[HelpURL("https://www.youtube.com/watch?v=3i-d6leI7Q4")]
public class Behaviour_DoorNew : MonoBehaviour, IBehaviour
{
    [SerializeField] private HingeJoint joint;
	[SerializeField] private MeshRenderer doorRenderer;

    private bool isSelected;
    private int leftDoor = 0;
    private Camera cam;
    private GameObject dragPointGameobject;
    private JointMotor motor;

    private void Start()
	{
		cam = PlayerController.Instance.Camera.GetComponent<Camera>();
	}

	public void Behaviour(bool isInteracting, bool isInspecting)
	{
		if(isInteracting) 
		{
			isSelected = true;
		}
	}

	public bool IsInspectable()
	{
		return false;
	}

	public bool IsInteractable()
	{
		return true;
	}

	void Update()
	{
		if (isSelected)
		{
			motor = joint.motor;

			//Create drag point object for reference where players mouse is pointing
			if (dragPointGameobject == null)
			{
				dragPointGameobject = new GameObject("Ray door");
				dragPointGameobject.transform.parent = transform;
			}

			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(transform.position, PlayerController.Instance.Player.transform.position));
			dragPointGameobject.transform.rotation = transform.rotation;


			float delta = Mathf.Pow(Vector3.Distance(dragPointGameobject.transform.position, transform.position), 3);

			//Deciding if it is left or right door
			if (doorRenderer.localBounds.center.x > transform.localPosition.x)
			{
				leftDoor = -1;
			}
			else
			{
				leftDoor = 1;
			}

			//Applying velocity to door motor
			float speedMultiplier = 60000;
			if (Mathf.Abs(transform.parent.forward.z) > 0.5f)
			{
				if (dragPointGameobject.transform.position.x > transform.position.x)
				{
					motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
				}
				else
				{
					motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
				}
			}
			else
			{
				if (dragPointGameobject.transform.position.z > transform.position.z)
				{
					motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
				}
				else
				{
					motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
				}
			}
			joint.motor = motor;

			if (Input.GetMouseButtonUp(0))
			{
				isSelected = false;
				motor.targetVelocity = 0;
				joint.motor = motor;
				Destroy(dragPointGameobject);
			}
		}
	}
}