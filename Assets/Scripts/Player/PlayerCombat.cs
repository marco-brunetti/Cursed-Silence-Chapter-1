using UnityEngine;
using Enemies;
using SnowHorse.Utils;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerController _controller;
        private PlayerData _data;
        private PlayerCombatState _state;
        private System.Random _random;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;
            _random = new System.Random(Guid.NewGuid().GetHashCode());
            ChangeState(PlayerCombatState.Idle);
        }

        private void Update()
        {
            switch (_state)
            {
                case PlayerCombatState.Idle:
                    ManageIdle();
                    break;
                case PlayerCombatState.Attack:
                    ManageAttack();
                    break;
                case PlayerCombatState.Block:
                    ManageBlock();
                    break;
                case PlayerCombatState.Stunned:
                    ManageStunned();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ManageIdle()
        {
            if (Input.GetMouseButtonDown(0) && !_controller.InteractableInSight)
            {
                ChangeState(PlayerCombatState.Attack);
            }
        }

        private void ManageAttack()
        {
            //Add player animation

            var enemy = Raycaster.Cast<EnemyController>(new() { origin = _controller.Camera.position, direction = _controller.Camera.forward },
                                                        out Vector3 hitPoint,
                                                        maxDistance: _data.AttackDistance,
                                                        layerMask: _data.InteractLayer,
                                                        debugMode: true);

            if (enemy) enemy.DealDamage(_data.DamageAmount, 10);
            else Debug.Log("Player attack nothing");

            ChangeState(PlayerCombatState.Idle);
        }

        private void ManageBlock()
        {
            //Add block logic
            ChangeState(PlayerCombatState.Idle);
        }

        private void ManageStunned()
        {
            //Add stunned logic
            ChangeState(PlayerCombatState.Idle);
        }

        private void ChangeState(PlayerCombatState newState)
        {
            _state = newState;
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