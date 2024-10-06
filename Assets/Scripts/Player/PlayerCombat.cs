using UnityEngine;
using Enemies;
using SnowHorse.Utils;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerController _controller;
        private PlayerData _data;
        private CombatState _currentState;

        private float currentAttackTime;
        private bool isAttacking;
        private bool isBlocking;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;
        }

        public void Manage()
        {
            if(Input.GetMouseButton(0)) ChangeState(CombatState.Attack);
            if(Input.GetMouseButton(1)) ChangeState(CombatState.Block);

            switch (_currentState)
            {
                case CombatState.Idle:
                    break;
                case CombatState.Attack:
                    AttackState();
                    break;
                case CombatState.HeavyAttack:
                    break;
                case CombatState.Block:
                    break;
            }
        }

        private void AttackState()
        {
            if (!isAttacking)
            {
                if (currentAttackTime < _data.HeavyAttackLoadTime) currentAttackTime += Time.deltaTime;
            }


            if (Input.GetMouseButtonUp(0))
            {
                isAttacking = true;

                if (currentAttackTime < _data.LightAttackMaxTime)
                {
                    AttackEnemy(isHeavyAttack: false);
                }
                else if (currentAttackTime < _data.HeavyAttackLoadTime)
                {
                    //Cancel heavy attack?
                }
                else
                {
                    AttackEnemy(isHeavyAttack: true);
                }
            }

            /*TEST*/AttackEnemy(isHeavyAttack: true);
        }

        private void BlockState()
        {
            
        }

        private void AttackEnemy(bool isHeavyAttack)
        {
            var raycast = new RaycastData
            {
                Origin = _controller.Camera.position,
                Direction = _controller.Camera.forward,
                MaxDistance = _data.AttackDistance,
                LayerMask = _data.InteractLayer,
                Debug = true
            };

            var enemy = Raycaster.Find<EnemyController>(raycast, out Vector3 hitPoint);

            if (enemy)
            {
                var damage = isHeavyAttack ? _data.HeavyAttackDamage : _data.LightAttackDamage;
                var poiseDecrement = isHeavyAttack ? _data.HeavyAttackPoiseDecrement : _data.LightAttackPoiseDecrement;
                enemy.DealDamage(damage, poiseDecrement);
            }
            else 
            {
                Debug.Log("Player attack nothing");
            }

            Debug.DrawRay(raycast.Origin, raycast.Direction * raycast.MaxDistance);
        }

        private void ChangeState(CombatState newState)
        {
            _currentState = newState;
        }

        private enum CombatState
        {
            Idle,
            Attack,
            HeavyAttack,
            Block
        }
    }
}