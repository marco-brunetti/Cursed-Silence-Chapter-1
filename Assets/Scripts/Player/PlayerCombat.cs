using UnityEngine;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerController _controller;
        private PlayerData _data;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;
        }


        public void Manage()
        {
            if (Input.GetMouseButtonDown(0) && !_controller.InteractableInSight)
            {
                Attack();
            }
        }

        private void Attack()
        {
            Debug.Log("Player Attacking");

            //Add player animation

            Ray ray = new()
            {
                origin = _controller.Camera.position,
                direction = _controller.Camera.forward
            };

            if (Physics.Raycast(ray, out RaycastHit hit, _data.InteractDistance, _data.InteractLayer) && hit.collider.TryGetComponent(out EnemyController enemy))
            {
                enemy.DealDamage(_data.DamageAmount);
            }
        }
    }
}