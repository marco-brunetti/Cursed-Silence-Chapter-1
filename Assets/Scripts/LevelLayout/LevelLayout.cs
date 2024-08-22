using UnityEngine;

public class LevelLayout : MonoBehaviour
{
    [field: SerializeField] public int Id {  get; private set; }
    public bool CanDispose;

    [SerializeField] private GameObject LayoutTrigger1;
    [SerializeField] private GameObject LayoutTrigger2;

    private void OnDisable()
    {
        LayoutTrigger1.SetActive(false);
        LayoutTrigger2.SetActive(false);
    }
}