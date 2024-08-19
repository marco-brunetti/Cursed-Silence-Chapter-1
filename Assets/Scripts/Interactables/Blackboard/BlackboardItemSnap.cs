using System;
using UnityEngine;

public class BlackboardItemSnap : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; } = 0;

    [NonSerialized] public bool Snapped;
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

        //var thisSnap = this;
        //var validSnap = other.TryGetComponent(out BlackboardItemSnap otherSnap) && otherSnap && otherSnap.Id == Id;
        //var isOnBlackboard = validSnap && _controller && _controller.BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
        //var validItem = isOnBlackboard && BlackboardItem == _controller.CurrentItem && BlackboardItem.Orientation == otherSnap.BlackboardItem.Orientation;

        //if (validItem)
        //{
        //    //thisSnap.Snapped = true;
        //    //otherSnap.Snapped = true;

        //    thisSnap.BlackboardItem.RegisterSnap(isSnapped:true, thisSnap);
        //    otherSnap.BlackboardItem.RegisterSnap(isSnapped: true, otherSnap);

        //    print($"snapped {gameObject.name}");
        //    _controller.CancelHold();
        //    var parentOffset = transform.parent.transform.position - transform.position;
        //    transform.parent.transform.position = otherSnap.transform.position + parentOffset;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (!SnapRegistered(isSnapping: false, other)) return;

        //var thisSnap = this;
        //var validSnap = other.TryGetComponent(out BlackboardItemSnap otherSnap) && otherSnap && otherSnap.Id == Id;
        //var isOnBlackboard = validSnap && _controller && _controller.BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
        //var validItem = isOnBlackboard && BlackboardItem == _controller.CurrentItem && BlackboardItem.Orientation == otherSnap.BlackboardItem.Orientation;

        //if (validItem)
        //{
        //    //thisSnap.Snapped = false;
        //    //otherSnap.Snapped = false;

        //    thisSnap.BlackboardItem.RegisterSnap(isSnapped: false, thisSnap);
        //    otherSnap.BlackboardItem.RegisterSnap(isSnapped: false, otherSnap);

        //    print($"unsnapped {gameObject.name}");
        //    //_controller.CancelHold();
        //    //var parentOffset = transform.parent.transform.position - transform.position;
        //    //transform.parent.transform.position = otherSnap.transform.position + parentOffset;
        //}
    }

    private bool SnapRegistered(bool isSnapping, Collider other)
    {
        var thisSnap = this;
        var validSnap = other.TryGetComponent(out BlackboardItemSnap otherSnap) && otherSnap && otherSnap.Id == Id;
        var isOnBlackboard = validSnap && _controller && _controller.BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
        var validItem = isOnBlackboard && BlackboardItem == _controller.CurrentItem && BlackboardItem.Orientation == otherSnap.BlackboardItem.Orientation;

        if (validItem)
        {
            thisSnap.BlackboardItem.RegisterSnap(isSnapping, thisSnap);
            otherSnap.BlackboardItem.RegisterSnap(isSnapping, otherSnap);
        }

        return validItem;
    }
}