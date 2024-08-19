using UnityEngine;

public class BlackboardItemSnap : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; } = 0;
    [field:SerializeField] public bool isBaseSnapPoint { get; private set; }
    public BlackboardItem BlackboardItem { get; private set; }

    private BlackboardController _controller;

    private void Awake()
    {
        BlackboardItem = transform.parent.GetComponent<BlackboardItem>();
    }

    private void Start()
    {
        _controller = BlackboardController.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!SnapRegistered(isSnapping:true, other)) return;

        _controller.CancelHold();
        var parentOffset = transform.parent.transform.position - transform.position;
        transform.parent.transform.position = other.transform.position + parentOffset;
    }

    private void OnTriggerExit(Collider other)
    {
        SnapRegistered(isSnapping: false, other);
    }

    private bool SnapRegistered(bool isSnapping, Collider other)
    {
        if(other.TryGetComponent(out BlackboardItemSnap otherSnap))
        {
            var thisSnap = this;
            var validSnap = otherSnap.Id == Id && (thisSnap.isBaseSnapPoint || otherSnap.isBaseSnapPoint);
            var isOnBlackboard = _controller.BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
            var validItem = BlackboardItem == _controller.CurrentItem && BlackboardItem.Orientation == otherSnap.BlackboardItem.Orientation;

            if (validSnap && validItem && isOnBlackboard)
            {
                thisSnap.BlackboardItem.RegisterSnap(isSnapping, thisSnap);
                otherSnap.BlackboardItem.RegisterSnap(isSnapping, otherSnap);

                return true;
            }
        }

        return false;
    }
}