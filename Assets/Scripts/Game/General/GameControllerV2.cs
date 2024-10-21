using System;
using System.Collections.Generic;
using UnityEngine;
using Layouts;
using SnowHorse.Systems;

namespace Game.General
{
    public class GameControllerV2 : MonoBehaviour
    {
        private List<GameObject> activeEnemies = new();
        [NonSerialized] public Transform PlayerTransform;
        private string currentLayoutStyle = "style0";
        public static GameControllerV2 Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);

            LayoutManager.LayoutStyleChanged += OnLayoutStyleChanged;
        }

        public void AddActiveEnemy(GameObject enemy)
        {
            if(!activeEnemies.Contains(enemy))
            {
                if(activeEnemies.Count == 0) SetCurrentMusic("fight");
                activeEnemies.Add(enemy);
            }
        }

        public void RemoveActiveEnemy(GameObject enemy)
        {
            if(activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
            if(activeEnemies.Count == 0) SetCurrentMusic(currentLayoutStyle);
        }

        public void OnLayoutStyleChanged(object sender, string style)
        {
            if(string.Equals(currentLayoutStyle, style)) return;
            currentLayoutStyle = style;
            SetCurrentMusic(style);
        }

        public void SetCurrentMusic(string style)
        {
            AudioManager.Instance.ActivateMixerMusic(style);
        }
    }
}