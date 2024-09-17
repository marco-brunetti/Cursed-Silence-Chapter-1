using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Layouts
{
    public class LevelLayout : MonoBehaviour
    {
        [field: SerializeField] public LayoutType Type { get; private set; }

        [field: SerializeField] public List<Vector3> NextLayoutOffsets { get; private set; }
        [field: SerializeField] public List<Vector3> NextLayoutRotations { get; private set; }

        [SerializeField] private GameObject entranceDoor;
        [SerializeField] private List<Behaviour_DoorNew> doors;
        [SerializeField] private LayoutData layoutData;

        [SerializeField] private Transform[] smallAnchors;
        [SerializeField] private Transform[] mediumAnchors;
        [SerializeField] private Transform[] largeAnchors;

        [Header("Style")] [SerializeField] private MeshRenderer[] wallRenderers;
        [SerializeField] private MeshRenderer[] doorWallRenderers;
        [SerializeField] private MeshRenderer[] windowWallRenderers;
        [SerializeField] private MeshRenderer[] ceilingRenderers;
        [SerializeField] private MeshRenderer[] floorRenderers;

        [NonSerialized] public int MapIndex = -1;
        [NonSerialized] public List<LayoutItem> ItemList = new();

        private List<Vector3> initialDoorRotations = new();

        public void Setup(int mapIndex, List<LayoutType> nextLayoutShapes, bool isEndOfZone,
            params LevelItem[] decorators)
        {
            MapIndex = mapIndex;
            SetDoorActions(nextLayoutShapes, isEndOfZone);
        }

        public bool HasDoors()
        {
            return doors != null && doors.Count > 0;
        }

        public void GetAnchors(out List<Transform> smallAnchors, out List<Transform> mediumAnchors,
            out List<Transform> largeAnchors)
        {
            smallAnchors = new(this.smallAnchors);
            mediumAnchors = new(this.mediumAnchors);
            largeAnchors = new(this.largeAnchors);
        }

        public void EntranceDoorEnabled(bool enabled)
        {
            entranceDoor.SetActive(enabled);
        }

        private void SetDoorActions(List<LayoutType> nextLayoutShapes, bool isEndOfZone)
        {
            entranceDoor.SetActive(false);
            if (doors == null || doors.Count == 0) return;

            for (int i = 0; i < doors.Count; i++)
            {
                initialDoorRotations.Add(doors[i].transform.localEulerAngles);

                if (nextLayoutShapes == null || nextLayoutShapes.Count == 0 || i >= nextLayoutShapes.Count ||
                    (nextLayoutShapes[i] == LayoutType.None))
                {
                    doors[i].SetDoorState(DoorState.Locked);
                    continue;
                }

                if (i < nextLayoutShapes.Count)
                {
                    var nextShape = nextLayoutShapes[i];
                    var offset = NextLayoutOffsets[i];
                    var rotation = Quaternion.Euler(NextLayoutRotations[i]);
                    UnityAction action = () =>
                        LayoutManager.Instance.ActivateLayout(previousLayout: transform, nextShape, offset, rotation);

                    //If end of zone, start deactivation process of previous zone
                    if (isEndOfZone && i == nextLayoutShapes.Count - 1)
                    {
                        var manager = LayoutManager.Instance;
                        manager.StartCoroutine(manager.DeactivateLevelLayouts());
                    }

                    doors[i].SetDoorState(DoorState.Closed);
                    doors[i].SetDoorAction(action);
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].transform.localEulerAngles = initialDoorRotations[i];
            }

            initialDoorRotations.Clear();
        }
    }
}