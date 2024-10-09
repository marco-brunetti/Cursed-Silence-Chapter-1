using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEmily : Enemy
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Renderer[] particleRenderers;

    private bool canDie;
    private bool isVulnerable;
    private bool isReacting;

    private List<Renderer> invisibleRenderers = new();

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Block()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override void Idle()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void React()
    {
        throw new System.NotImplementedException();
    }

    public override void SpecialAttack()
    {
        throw new System.NotImplementedException();
    }
}
