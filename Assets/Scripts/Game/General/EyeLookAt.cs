using System;
using UnityEngine;

public class EyeLookAt : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        transform.LookAt(target.position);
    }
}
