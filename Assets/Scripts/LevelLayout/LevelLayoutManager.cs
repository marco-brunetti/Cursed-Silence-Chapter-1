using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
    [SerializeField] private LevelLayout[] layoutPrefabs;

    [SerializeField] private List<LevelLayout> activeLayouts = new();
    [SerializeField] private LevelLayout currentLayout;

    public static LevelLayoutManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        StartCoroutine(DeactivateLevelLayouts());
    }

    public void SetCurrentLayout(LevelLayout newCurrent)
    {
        MarkForDeactivation(exceptions: new() { currentLayout, newCurrent });

        currentLayout = newCurrent;
    }

    public void ActivateLayout(int nextId, Vector3 position, Vector3 rotation)
    {
        var levelLayout = activeLayouts.FirstOrDefault(x => x.Id == nextId && !x.gameObject.activeInHierarchy) as LevelLayout;

        if(!levelLayout)
        {
            levelLayout = Instantiate(Array.Find(layoutPrefabs, e => e.Id == nextId));
            activeLayouts.Add(levelLayout);
        }

        levelLayout.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        levelLayout.gameObject.SetActive(true);

        MarkForDeactivation(exceptions: new() { currentLayout, levelLayout });

        currentLayout = levelLayout;
    }

    private void MarkForDeactivation(List<LevelLayout> exceptions)
    {

        foreach(var layout in activeLayouts)
        {
            if(exceptions.Contains(layout)) layout.canDispose = false;
            else layout.canDispose = true;
        }
    }

    private IEnumerator DeactivateLevelLayouts()
    {
        yield return new WaitForSecondsRealtime(1);

        foreach(var levelLayout in activeLayouts)
        {
            if(levelLayout.canDispose)
            {
                levelLayout.gameObject.SetActive(false);
            }
        }

        StartCoroutine(DeactivateLevelLayouts());
    }
}