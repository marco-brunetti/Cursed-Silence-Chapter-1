using UnityEngine;
using Enemies;
using SnowHorse.Utils;

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
                Cover();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Attack()
        {
            //Add player animation

            var enemy = Raycaster.Cast<EnemyController>(new() { origin = _controller.Camera.position, direction = _controller.Camera.forward },
                                                        out Vector3 hitPoint,
                                                        maxDistance: _data.AttackDistance,
                                                        layerMask: _data.InteractLayer);

            if(enemy) enemy.DealDamage(_data.DamageAmount);
            else Debug.Log("Player attack nothing");
        }

        private void Cover()
        {
            
        }
    }

    public enum PlayerCombatState
    {
        Idle,
        Attack,
        Block,
        Stunned
    }
}