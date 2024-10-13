using System.Collections;
using System.Collections.Generic;

namespace Enemies
{
    public class EnemyEmily : Enemy
    {
        private string SpecialAttack2AnimKey = "special_attack_2_part_0";

        protected override IEnumerator AttackingPlayer()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name, };
            if (hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if (hasSpecialAttack)
            {
                attackKeysList.Add(data.SpecialAttackAnim.name);
                attackKeysList.Add(SpecialAttack2AnimKey);
            }
                

            if (!attackKeysList.Contains(animation.CurrentKey))
            {
                animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                yield return null;
            }

            while (attackKeysList.Contains(animation.CurrentKey))
            {
                if (changeNextAttack) SetRandomAttack();
                yield return null;
            }

            attack = null;
        }


        protected override void SetRandomAttack()
        {

            if (animation.CurrentKey == data.AttackAnim.name)
            {
                var p = 0;
                if (hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);

                if (hasHeavyAttack && hasSpecialAttack)
                {
                    //Added second special attack for Emily
                    if (p < data.SpecialAttackProbability)
                    {
                        var sp = random.Next(0, 100);
                        if (sp < 50) animation.SetState(data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                        else animation.SetState(SpecialAttack2AnimKey, rootTransformForLook: transform, lookTarget: player);
                    }
                        
                    else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasHeavyAttack)
                {
                    if (p < data.HeavyAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasSpecialAttack)
                {
                    if (p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
            }
            else
            {
                animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
            }

            changeNextAttack = false;
        }
    }
}
