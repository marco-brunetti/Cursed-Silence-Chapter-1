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
            base.Move();
            //animation.SetState(Data.MoveAnim.name);
        }
    }
}
