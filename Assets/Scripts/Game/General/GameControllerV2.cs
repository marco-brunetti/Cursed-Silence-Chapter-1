using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.General
{
    public class GameControllerV2 : MonoBehaviour
    {
        private List<GameObject> activeEnemies = new();
        [NonSerialized] public Transform PlayerTransform;
        public CurrentLayoutStyle CurrentLayoutStyle { get; private set; } = CurrentLayoutStyle.Style0;
        public static GameControllerV2 Instance;
        
        public static EventHandler EnemiesActive;
        public static EventHandler EnemiesInactive;

        public static EventHandler<CurrentLayoutStyle> LayoutStyleChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void AddActiveEnemy(GameObject enemy)
        {
            if(!activeEnemies.Contains(enemy))
            {
                if(activeEnemies.Count == 0) EnemiesActive?.Invoke(null, null);
                activeEnemies.Add(enemy);
            }
        }

        public void RemoveActiveEnemy(GameObject enemy)
        {
            if(activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
            if(activeEnemies.Count == 0) EnemiesInactive?.Invoke(null, null);
        }

        public void SetCurrentStyle(CurrentLayoutStyle style)
        {
            if(CurrentLayoutStyle == style) return;
            CurrentLayoutStyle = style;
            LayoutStyleChanged?.Invoke(null, style);
        }
    }
}

public enum CurrentLayoutStyle
{
    Style0,
    Style1,
    Style2,
    Style3,
    Style4
}