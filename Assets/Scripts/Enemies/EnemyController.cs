using System.Threading.Tasks;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    private bool isEngaging;
    private bool canRecieveDamage;

    public void DealDamage(int damageAmount)
    {

    }

    private async void DamageControl()
    {
        await WaitForSecondsAsync(4);

        if (isEngaging) DamageControl();
    }

    async Task WaitForSecondsAsync(float delay)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));
    }
}