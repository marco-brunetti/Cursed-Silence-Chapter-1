using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectGUID : MonoBehaviour
{
    [field: SerializeField] public string Id { get; private set; }

    private float delayInSeconds = 30;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += Generate;
#endif
    }

    private void Generate()
    {
#if UNITY_EDITOR

        if (EditorApplication.timeSinceStartup < delayInSeconds && (string.IsNullOrEmpty(Id)))
        {
            int timeLeft = (int)delayInSeconds - (int)EditorApplication.timeSinceStartup;
            Debug.Log($"Will attempt {gameObject.name} GUID update in: {timeLeft}.");
        }
        else
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
                Debug.Log($"Generated new GUID for {gameObject.name}. If this object existed previously, save file deletion is recommended.");
            }

            EditorApplication.update -= Generate;
        }
#endif
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.update -= Generate;
#endif
    }
}