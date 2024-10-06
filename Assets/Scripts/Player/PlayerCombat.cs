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

        private float currentAttackLoadTime;
        private float currentLightAttackCooldown;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;
        }

        public void Manage()
        {
            if(Input.GetMouseButtonDown(0)) ChangeState(CombatState.Attack);
            if(Input.GetMouseButtonDown(1)) ChangeState(CombatState.Block);

            switch (_currentState)
            {
                case CombatState.Attack:
                    AttackState();
                    break;
                case CombatState.Block:
                    break;
                case CombatState.None:
                    break;
            }
        }

        private void AttackState()
        {
            currentAttackLoadTime += Time.deltaTime;

            if (Input.GetMouseButtonUp(0))
            {
                if (currentAttackLoadTime <= _data.LightAttackMaxTime)
                {
                    AttackEnemy(isHeavyAttack: false);
                }
                else if (currentAttackLoadTime < _data.HeavyAttackLoadTime)
                {
                    //Cancel heavy attack?
                    Debug.Log($"Player HEAVY ATTACK CANCELLED.");
                }
                else
                {
                    AttackEnemy(isHeavyAttack: true);
                }

                ChangeState(CombatState.None);
            }
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

            var enemy = Raycaster.Find<EnemyController>(raycast).HitObject;

            if (enemy)
            {
                var damage = isHeavyAttack ? _data.HeavyAttackDamage : _data.LightAttackDamage;
                var poiseDecrement = isHeavyAttack ? _data.HeavyAttackPoiseDecrement : _data.LightAttackPoiseDecrement;
                enemy.DealDamage(damage, poiseDecrement);
            }

            string targetName = enemy ? enemy.name.ToUpper() : "NONE"; 

            if (isHeavyAttack) Debug.Log($"Player used HEAVY ATTACK against: {targetName}");
            else Debug.Log($"Player used LIGHT ATTACK against: {targetName}");

            Debug.DrawRay(raycast.Origin, raycast.Direction * raycast.MaxDistance);
        }

        private void ChangeState(CombatState newState)
        {
            currentAttackLoadTime = 0;
            _currentState = newState;
        }

        private enum CombatState
        {
            Attack,
            Block,
            None
        }
    }
}