using System.Collections;
using UnityEngine;

public class Behaviour_DeactivateColliders : MonoBehaviour, IBehaviour
{
    [SerializeField] private Collider[] _colliders;
    [SerializeField] private float _delay;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        StartCoroutine(Deactivate());
    }

    public bool IsInspectable()
    {
        return false;
    }

    public bool IsInteractable()
    {
        return true;
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(_delay);

        for(int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
    }
}
