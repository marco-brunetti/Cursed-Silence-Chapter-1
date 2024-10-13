namespace Enemies
{
    public class EnemyEmily : Enemy
    {
        protected override void SetRandomAttack()
        {





            /*if (animation.CurrentKey == data.AttackAnim.name)
            {
                var p = 0;
                if (hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);

                if (hasHeavyAttack && hasSpecialAttack)
                {
                    if (p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
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

            changeNextAttack = false;*/
        }
    }
}
