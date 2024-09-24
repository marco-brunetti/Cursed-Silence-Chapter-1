using UnityEngine;
using UnityEngine.Events;
using Interactables.Behaviours;

public class BlackboardItemSnap : MonoBehaviour
{
	[field:SerializeField] public int Id { get; private set; } = 0;
	[field:SerializeField] public bool isBaseSnapPoint { get; private set; }
    public BlackboardItem BlackboardItem { get; private set; }
    private UnityAction<bool, BlackboardItemSnap, BlackboardItemSnap> snapAction;

	public void SetSnapAction(UnityAction<bool, BlackboardItemSnap, BlackboardItemSnap> action, BlackboardItem item)
	{
		snapAction = action;
		BlackboardItem = item;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out BlackboardItemSnap otherSnap))
		{
			snapAction(true, this, otherSnap);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out BlackboardItemSnap otherSnap))
		{
			snapAction(false, this, otherSnap);
		}
	}
}