using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelLayoutManager : MonoBehaviour
{
    [SerializeField] private LevelLayout[] layoutPrefabs;
    [SerializeField] private LevelLayout currentLayout;
    [SerializeField] private TextAsset mapJson;

    private HashSet<LevelLayout> activeLayouts = new();
    private Queue<LevelLayout> deactivateQueue = new();
    private List<Layout> mapLayouts = new();
    private LayoutMap map;

    private int currentLayoutIndex = -1;

    public static LevelLayoutManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        SetupMap();
    }

    private void Start()
    {
        if (currentLayout) activeLayouts.Add(currentLayout);
        StartCoroutine(DeactivateLevelLayouts());
    }

    private void SetupMap()
    {
        map = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

        for(int i = 0; i < map.LayoutStates.Count; i++)
        {
            mapLayouts.Add(map.LayoutStates[i]);

            if(currentLayoutIndex == -1 && map.LayoutStates[i].enable) currentLayoutIndex = i;
        }

        //Sets up first level
        currentLayout.Setup(mapLayouts[currentLayoutIndex].style, doorAction: ()=> ActivateLayout(mapLayouts[currentLayoutIndex+1].id,
            currentLayout.transform.position + currentLayout.NextLayoutOffset, Quaternion.Euler(currentLayout.NextLayoutRotation)), null);
    }

    public void SetCurrentLayout(LevelLayout newCurrent)
    {
        //MarkForDeactivation(exceptions: new() { currentLayout, newCurrent });
        currentLayout = newCurrent;
    }

    public void ActivateLayout(int id, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
    {
        var levelLayout = activeLayouts.FirstOrDefault(x => x.Id == id && !x.gameObject.activeInHierarchy);

        if (!levelLayout)
        {
            levelLayout = Instantiate(Array.Find(layoutPrefabs, e => e.Id == id));
            activeLayouts.Add(levelLayout);
        }

        levelLayout.transform.SetPositionAndRotation(position, rotation);
        levelLayout.Setup(LayoutStyle.Style2, doorAction: null, decorators: decorators);
        levelLayout.gameObject.SetActive(true);

        foreach (var decorator in decorators)
        {
            decorator.ApplyDecorator(levelLayout);
        }

        //MarkForDeactivation(exceptions: new() { currentLayout, levelLayout });
        currentLayoutIndex++;
        currentLayout = levelLayout;

        UnityAction action = null;

        if(currentLayoutIndex < mapLayouts.Count - 1)
        {
            action = () => {
                ActivateLayout(mapLayouts[currentLayoutIndex + 1].id,
            currentLayout.transform.position + currentLayout.NextLayoutOffset, Quaternion.Euler(currentLayout.NextLayoutRotation));
            };
        }
        else if(currentLayoutIndex == mapLayouts.Count - 1)
        {
            Debug.Log("No more levels");
        }

        currentLayout.Setup(mapLayouts[currentLayoutIndex].style, doorAction: action, null);
    }

    private void MarkForDeactivation(List<LevelLayout> exceptions)
    {
        foreach (var layout in activeLayouts)
        {
            if (exceptions.Contains(layout))
            {
                layout.CanDispose = false;
            }
            else
            {
                layout.CanDispose = true;
                deactivateQueue.Enqueue(layout);
            }
        }
    }

    private IEnumerator DeactivateLevelLayouts()
    {
        while (true)
        {
            if (deactivateQueue.Count > 0)
            {
                var layout = deactivateQueue.Dequeue();
                if (layout.CanDispose)
                {
                    layout.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }
}

public record LayoutMap
{
    [JsonProperty("layoutStates")]
    public List<Layout> LayoutStates;
}