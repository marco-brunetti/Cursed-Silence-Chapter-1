using System;
using Enemies;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [NonSerialized] public EnemyController Controller;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Controller.PlayerDetected(detector: this);
        }
    }
}
