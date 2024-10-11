using System;
using UnityEngine;

public class GameControllerV2 : MonoBehaviour
{
    [NonSerialized] public Transform PlayerTransform;
    public static GameControllerV2 Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }


    public void DamageEnemy(GameObject enemy, int damage, int poiseDecrement)
    {

    }
}
