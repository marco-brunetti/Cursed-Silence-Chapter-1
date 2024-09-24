using UnityEngine;
using Interactables.Behaviours;

public class OnTriggerEnterBehaviour : MonoBehaviour
{
    [SerializeField] private string _colliderTag = "Player";
    [SerializeField] private GameObject[] _behaviours;

    [SerializeField] private bool _deactivateTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_colliderTag))
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
            }
        }

        if(_deactivateTrigger)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
