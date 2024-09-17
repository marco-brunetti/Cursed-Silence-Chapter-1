using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Layouts
{
    public class LayoutManager : MonoBehaviour
    {
        [SerializeField] private TextAsset mapJson;
        [SerializeField] private LayoutData layoutData;
        [SerializeField] private LevelItemManager itemManager;

        private int currentIndex;
        private LayoutMap savedMap;
        private List<Layout> loadedMap = new();
        private LevelLayout mainLevel;
        private HashSet<LevelLayout> layoutPool = new();
        private Queue<LevelLayout> deactivateQueue = new();

        public static LayoutManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);

            savedMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

            var mainLevelPrefab =
                layoutData.prefabs.FirstOrDefault(x => x != null && x.Type == LayoutType.MainLevelStyle0);

            if (mainLevelPrefab)
            {
                mainLevel = Instantiate(mainLevelPrefab);
                mainLevel.gameObject.SetActive(false);
            }

            for (int i = 0; i < savedMap.Layouts.Count; i++)
            {
                if (savedMap.Layouts[i].enable) loadedMap.Add(savedMap.Layouts[i]);
            }

            var currentMapLayout = loadedMap[currentIndex];
            ActivateLayout(null, currentMapLayout.nextShapes[0], Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void ActivateLayout(Transform previousLayout, LayoutType nextType, Vector3 position, Quaternion rotation)
        {
            if (currentIndex >= loadedMap.Count)
            {
                Debug.Log("End of map.");
                return;
            }

            LevelLayout newLayout = null;

            if (nextType == LayoutType.MainLevelStyle0 && mainLevel)
            {
                newLayout = mainLevel;
            }
            else
            {
                newLayout = layoutPool.FirstOrDefault(x =>
                    x != null && x.Type == nextType && !x.gameObject.activeInHierarchy);

                if (!newLayout)
                {
                    var prefab = layoutData.prefabs.FirstOrDefault(x => x != null && x.Type == nextType);

                    if (prefab)
                    {
                        newLayout = Instantiate(prefab);
                        layoutPool.Add(newLayout);
                    }
                    else
                    {
                        Debug.Log($"Level type {nextType} not found.");
                        return;
                    }
                }
            }

            if (previousLayout) newLayout.transform.parent = previousLayout;
            newLayout.transform.SetLocalPositionAndRotation(position, rotation);
            newLayout.transform.parent = null;
            newLayout.gameObject.SetActive(true);

            var i = currentIndex;
            var mapLayout = loadedMap[i];
            var isEndOfZone = i < (loadedMap.Count - 1) && mapLayout.zone != loadedMap[i + 1].zone;

            newLayout.Setup(i, mapLayout.nextShapes, isEndOfZone, decorators: null);
            if (i == 0) newLayout.EntranceDoorEnabled(true);
            newLayout.ItemList = mapLayout.items;
            itemManager.FillItems(newLayout);
            if (newLayout.HasDoors()) currentIndex++;
        }

        public IEnumerator DeactivateLevelLayouts()
        {
            while (deactivateQueue.Count > 0)
            {
                var layout = deactivateQueue.Dequeue();
                layout.gameObject.SetActive(false);
                ActivateZoneEntranceDoor(layout.MapIndex + 1);
                itemManager.RemoveFrom(layout);
                layout.MapIndex = -1;
                yield return null;
            }

            MarkForDeactivation();
        }

        private void ActivateZoneEntranceDoor(int index)
        {
            if (deactivateQueue.Count == 0 && index < loadedMap.Count)
            {
                layoutPool.FirstOrDefault(x => x.MapIndex == index && x.gameObject.activeInHierarchy)
                    .EntranceDoorEnabled(true);
            }
        }

        private void MarkForDeactivation()
        {
            foreach (var layout in layoutPool)
            {
                if (layout.MapIndex <= currentIndex) deactivateQueue.Enqueue(layout);
            }
        }
    }
}