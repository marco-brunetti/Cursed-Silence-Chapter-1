namespace Enemies
{
    public class EnemySpider : Enemy
    {

        public override void Start()
        {
            base.Start();
            isVulnerable = true;
        }
        protected override void Move()
        {
            animation.SetState(Data.MoveAnim.name, lookTarget: player, moveTarget: player, moveSpeed: 1f);
        }
    }
}
