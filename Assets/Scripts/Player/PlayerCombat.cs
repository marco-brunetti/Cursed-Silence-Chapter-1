using UnityEngine;
using Enemies;
using SnowHorse.Utils;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerController _controller;
        private PlayerData _data;

        private bool isAttacking;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;
        }

        public void Manage()
        {
            if(!isAttacking && Input.GetMouseButton(0))
            {
                isAttacking = true;

            }
        }

        private void ManageAttack()
        {
            var raycast = new RaycastData
            {
                Origin = _controller.Camera.position,
                Direction = _controller.Camera.forward,
                MaxDistance = _data.AttackDistance,
                LayerMask = _data.InteractLayer
            };

            var enemy = Raycaster.Find<EnemyController>(raycast, out Vector3 hitPoint);

            if (enemy) enemy.DealDamage(_data.DamageAmount, 10);
            else Debug.Log("Player attack nothing");
        }
    }
}