using System;
using UnityEngine;

public class BlackboardItemSnap : MonoBehaviour
{
    [field:SerializeField] public int Id {  get; private set; } = 0;

    public BlackboardItem BlackboardItem {  get; private set; }

    private void Awake()
    {
        BlackboardItem = transform.parent.GetComponent<BlackboardItem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var validSnap = other.TryGetComponent(out BlackboardItemSnap snap) && snap && snap.Id == Id;
        var validItem = validSnap && BlackboardItem == BlackboardController.Instance.CurrentItem && BlackboardItem.Orientation == snap.BlackboardItem.Orientation;

        if (validItem)
        {
            print("snapped");
            BlackboardController.Instance.CancelHold();
            var parentOffset = transform.parent.transform.position - transform.position;
            transform.parent.transform.position = snap.transform.position + parentOffset;
        }
    }
}