using System.Collections.Generic;
using UnityEngine;
using SnowHorse.Systems;
using Enemies;
using Player;

namespace Game.General
{
    public static class GameControllerV2
    {
        public static List<Enemy> activeEnemies = new();
        public static Transform playerTransform;
        public static string CurrentLayoutStyle { get; private set; }// = "style0";
        //private Transform playerTransform;
        public static List<Enemy> enemyWaitingList = new();

        public static void ActiveCursor(bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
        }
        
        public static void SetLevelStyle(string newStyle) => CurrentLayoutStyle = newStyle;

        public static void SetCurrentMusic(string style, float blendTime = 0)
        {
            AudioManager.Instance.PlayMusic(style, blendTime);
        }

        public static void PlayAudio(string id, AudioClip clip, float volume = 1f, float pitch = 1)
        {
            AudioManager.Instance.PlayAudio(id, clip, volume, pitch);
        }
    }
}