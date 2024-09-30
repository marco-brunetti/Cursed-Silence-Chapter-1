using UnityEngine;

public class OnTriggerEnterBehaviour : MonoBehaviour
{
    [SerializeField] private string _colliderTag = "Player";
    [SerializeField] private GameObject[] _behaviours;

    [SerializeField] private DeactivateType _deactivateType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_colliderTag))
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
            }
        }

        switch(_deactivateType)
        {
            case DeactivateType.None:
                return;
            case DeactivateType.Collider:
                GetComponent<Collider>().enabled = false;
                break;
            case DeactivateType.GameObject:
                gameObject.SetActive(false);
                break;
        }
    }

    private enum DeactivateType
    {
        None,
        Collider,
        GameObject
    }
}
