using System;
using UnityEngine;
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
        private bool attackQueued;

        private float animationTriggerVelocity = 1;

        public static EventHandler<DamageEnemyEventArgs> DamageEnemy;

        private void Start()
        {
            if (!_controller) _controller = PlayerController.Instance;
            _data = _controller.PlayerData;

            _currentState = CombatState.None;
        }

        public void Manage(float currentVelocity)
        {
            if (_controller.InteractableInSight != null)
            {
                NormalState(currentVelocity);
                return;
            }
            
            if (currentLightAttackCooldown > 0)
            {
                currentLightAttackCooldown -= Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0))
            {
                attackQueued = true;
                ChangeState(CombatState.Attack);
            }

            if (Input.GetMouseButtonDown(1) && !PlayerController.Instance.IsInspecting) ChangeState(CombatState.Block);


            switch (_currentState)
            {
                case CombatState.Attack:
                    AttackState();
                    break;
                case CombatState.Block:
                    attackQueued = false;
                    BlockState();
                    break;
                default:
                    attackQueued = false;
                    NormalState(currentVelocity);
                    break;
            }
        }

        private void NormalState(float currentVelocity)
        {
            if (currentVelocity > animationTriggerVelocity)
            {
                if(Input.GetKey(KeyCode.LeftShift)) _controller.Animation.Run();
                else _controller.Animation.Walk();
            }
            else _controller.Animation.Idle();
        }

        public void DealDamage()
        {
            attackQueued = false;
        }

        private void AttackState()
        {
            if (attackQueued && currentLightAttackCooldown <= 0)
            {
                currentAttackLoadTime += Time.deltaTime;

                if (Input.GetMouseButtonUp(0))
                {
                    if (currentAttackLoadTime <= _data.LightAttackMaxTime) AttackEnemy(isHeavyAttack: false);
                    else if (currentAttackLoadTime < _data.HeavyAttackLoadTime) Debug.Log($"Player HEAVY ATTACK CANCELLED.");
                    else AttackEnemy(isHeavyAttack: true);

                    currentLightAttackCooldown = _data.LightAttackCooldown;
                    attackQueued = false;
                    ChangeState(CombatState.None);
                }
            }
        }

        private void BlockState()
        {
            _controller.Animation.Block();
        }

        private void AttackEnemy(bool isHeavyAttack)
        {
            var rayData = new RaycastData
            {
                Origin = _controller.Camera.position,
                Direction = _controller.Camera.forward,
                MaxDistance = _data.AttackDistance,
                LayerMask = _data.InteractLayer,
                FindTag = "Enemy",
                //Debug = true
            };

            var enemy = Raycaster.FindWithTag<GameObject>(rayData)?.HitObject;

            if (enemy)
            {
                int damage;
                int poiseDecrement;
                if (isHeavyAttack)
                {
                    damage = _data.HeavyAttackDamage;
                    poiseDecrement = _data.HeavyAttackPoiseDecrement;
                    _controller.Animation.HeavyAttack();
                }
                else
                {
                    damage = _data.LightAttackDamage;
                    poiseDecrement = _data.LightAttackPoiseDecrement;
                    _controller.Animation.Attack();
                }

                DamageEnemy.Invoke(this, new DamageEnemyEventArgs(enemy, damage, poiseDecrement));
            }

            string targetName = enemy ? enemy.name.ToUpper() : "NONE";
            if (isHeavyAttack) Debug.Log($"Player used HEAVY ATTACK against: {targetName}");
            else Debug.Log($"Player used LIGHT ATTACK against: {targetName}");

            Debug.DrawRay(rayData.Origin, rayData.Direction * rayData.MaxDistance);
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

    public class DamageEnemyEventArgs : EventArgs
    {
        public readonly GameObject Enemy;
        public readonly int Damage;
        public readonly int PoiseDecrement;

        public DamageEnemyEventArgs(GameObject enemy, int damage, in int poiseDecrement)
        {
            Enemy = enemy;
            Damage = damage;
            PoiseDecrement = poiseDecrement;
        }
    }
}