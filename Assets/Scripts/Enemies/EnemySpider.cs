namespace Enemies
{
    public class EnemySpider : Enemy
    {
        protected override void Move()
        {
            animation.SetState(Data.MoveAnim.name, lookTarget: player, moveTarget: player, moveSpeed: 1f);
        }
    }
}
