using System;
using UnityEngine;

public class BlackboardItemSnap : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; } = 0;

    [NonSerialized] public bool Snapped;
    public BlackboardItem BlackboardItem { get; private set; }

    private void Awake()
    {
        BlackboardItem = transform.parent.GetComponent<BlackboardItem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var validSnap = other.TryGetComponent(out BlackboardItemSnap snap) && snap && snap.Id == Id;
        var isOnBlackboard = validSnap && BlackboardController.Instance.BlackboardItems.Contains(snap.BlackboardItem.gameObject);
        var validItem = isOnBlackboard && BlackboardItem == BlackboardController.Instance.CurrentItem && BlackboardItem.Orientation == snap.BlackboardItem.Orientation;

        if (validItem)
        {
            Snapped = true;
            snap.Snapped = true;

            print($"snapped{gameObject.name}");
            BlackboardController.Instance.CancelHold();
            var parentOffset = transform.parent.transform.position - transform.position;
            transform.parent.transform.position = snap.transform.position + parentOffset;
        }
    }
}