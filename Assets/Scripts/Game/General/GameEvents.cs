using System;
using UnityEngine;
using Enemies;
using Player;
using Layouts;

namespace Game.General
{
    public class GameEvents : MonoBehaviour
    {
        private void Awake()
        {
            LayoutManager.LayoutStyleChanged += OnLayoutStyleChanged;
            PlayerController.SetPlayerTransform += OnSetPlayerTransform;
            PlayerCombat.DamageEnemy += OnDamageEnemy;
            PlayerAnimation.Step += OnPlayerAudioPlay;
            Enemy.EnemyAwake += OnEnemyAwake;
            Enemy.AddActiveEnemy += OnAddActiveEnemy;
            Enemy.RemoveActiveEnemy += OnRemoveActiveEnemy;
        }

        private void Start()
        {
            OnLayoutStyleChanged(null, "style1");
            GameControllerV2.ActiveCursor(false);
        }

        private void OnLayoutStyleChanged(object sender, string style)
        {
            if(string.Equals(GameControllerV2.CurrentLayoutStyle, style)) return;
            GameControllerV2.SetLevelStyle(style);
            GameControllerV2.SetCurrentMusic(style);
        }

        private void OnSetPlayerTransform(object sender, Transform transform)
        {
            GameControllerV2.playerTransform = transform;
            if(GameControllerV2.enemyWaitingList.Count > 0) GameControllerV2.enemyWaitingList.ForEach(enemy => enemy.SetPlayerTransform(transform));
            GameControllerV2.enemyWaitingList.Clear();
        }

        private void OnEnemyAwake(object sender, Enemy enemy)
        {
            if(GameControllerV2.playerTransform) enemy.SetPlayerTransform(GameControllerV2.playerTransform);
            else if(!GameControllerV2.enemyWaitingList.Contains(enemy)) GameControllerV2.enemyWaitingList.Add(enemy);
        }

        private void OnAddActiveEnemy(object sender, Enemy enemy)
        {
            if(!GameControllerV2.activeEnemies.Contains(enemy))
            {
                if(GameControllerV2.activeEnemies.Count == 0) GameControllerV2.SetCurrentMusic("fight", blendTime: 1f);
                GameControllerV2.activeEnemies.Add(enemy);
            }
        }

        private void OnRemoveActiveEnemy(object sender, Enemy enemy)
        {
            if(GameControllerV2.activeEnemies.Contains(enemy)) GameControllerV2.activeEnemies.Remove(enemy);
            if(GameControllerV2.activeEnemies.Count == 0) GameControllerV2.SetCurrentMusic(GameControllerV2.CurrentLayoutStyle);
        }

        private void OnDamageEnemy(object sender, DamageEnemyEventArgs e)
        {
            var enemy = e.Enemy.GetComponent<Enemy>();
            if(GameControllerV2.activeEnemies.Contains(enemy))
            {
                enemy.Damage(e.Damage, e.PoiseDecrement);
            }
        }

        private void OnPlayerAudioPlay(object sender, PlayerAudioEventArgs e) => GameControllerV2.PlayAudio(e.Id, e.Clip, e.Volume, e.Pitch);
    }
}