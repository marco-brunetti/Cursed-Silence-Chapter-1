using System;
using System.Collections.Generic;
using UnityEngine;
using SnowHorse.Systems;
using Layouts;
using Enemies;
using Player;

namespace Game.General
{
    public class GameControllerV2 : MonoBehaviour
    {
        private List<Enemy> activeEnemies = new();
        [NonSerialized] public Transform PlayerTransform;
        private string currentLayoutStyle = "style0";
        private Transform playerTransform;
        private List<Enemy> enemyWaitingList = new();

        private void Awake()
        {
            LayoutManager.LayoutStyleChanged += OnLayoutStyleChanged;
            PlayerController.SetPlayerTransform += OnSetPlayerTransform;
            PlayerCombat.DamageEnemy += OnDamageEnemy;
            Enemy.EnemyAwake += OnEnemyAwake;
            Enemy.AddActiveEnemy += OnAddActiveEnemy;
            Enemy.RemoveActiveEnemy += OnRemoveActiveEnemy;
        }

        private void OnLayoutStyleChanged(object sender, string style)
        {
            if(string.Equals(currentLayoutStyle, style)) return;
            currentLayoutStyle = style;
            SetCurrentMusic(style);
        }

        private void OnSetPlayerTransform(object sender, Transform transform)
        {
            PlayerTransform = transform;
            if(enemyWaitingList.Count > 0) enemyWaitingList.ForEach(enemy => enemy.SetPlayerTransform(transform));
            enemyWaitingList.Clear();
        }

        private void OnEnemyAwake(object sender, Enemy enemy)
        {
            if(playerTransform) enemy.SetPlayerTransform(playerTransform);
            else if(!enemyWaitingList.Contains(enemy)) enemyWaitingList.Add(enemy);
        }

        private void OnAddActiveEnemy(object sender, Enemy enemy)
        {
            if(!activeEnemies.Contains(enemy))
            {
                if(activeEnemies.Count == 0) SetCurrentMusic("fight", blendTime: 1f);
                activeEnemies.Add(enemy);
            }
        }

        private void OnRemoveActiveEnemy(object sender, Enemy enemy)
        {
            if(activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
            if(activeEnemies.Count == 0) SetCurrentMusic(currentLayoutStyle);
        }

        private void OnDamageEnemy(object sender, DamageEnemyEventArgs e)
        {
            var enemy = e.Enemy.GetComponent<Enemy>();
            if(activeEnemies.Contains(enemy))
            {
                enemy.Damage(e.Damage, e.PoiseDecrement);
            }
        }

        private void SetCurrentMusic(string style, float blendTime = 0)
        {
            AudioManager.Instance.PlayMusic(style, blendTime);
        }
    }
}