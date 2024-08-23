using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
    [SerializeField] private LevelLayout[] layoutPrefabs;
    [SerializeField] private LevelLayout currentLayout;

    private HashSet<LevelLayout> activeLayouts = new();
    private Queue<LevelLayout> deactivateQueue = new Queue<LevelLayout>();

    public static LevelLayoutManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        if(currentLayout) activeLayouts.Add(currentLayout);
        StartCoroutine(DeactivateLevelLayouts());
    }

    public void SetCurrentLayout(LevelLayout newCurrent)
    {
        MarkForDeactivation(exceptions: new() { currentLayout, newCurrent });
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
        levelLayout.gameObject.SetActive(true);

        foreach (var decorator in decorators)
        {
            decorator.ApplyDecorator(levelLayout);
        }

        MarkForDeactivation(exceptions: new() { currentLayout, levelLayout });

        currentLayout = levelLayout;
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