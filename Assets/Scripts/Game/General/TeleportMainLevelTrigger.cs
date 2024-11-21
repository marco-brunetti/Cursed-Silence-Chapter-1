using Game.General;
using UnityEngine;

public class TeleportMainLevelTrigger : MonoBehaviour
{
    [SerializeField] private TeleportMainLevelObjects mainLevelTeleport;
    [SerializeField] private bool teleportPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(teleportPlayer) mainLevelTeleport.UpdatePlayerAndObjectsPosition();
            else mainLevelTeleport.UpdateObjectsPosition();
            gameObject.SetActive(false);
        }
    }
}
