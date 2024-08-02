using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Behaviour_LevelLoader : MonoBehaviour, IBehaviour
{
    [SerializeField] private string _scene;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(_scene != "")
        {
            SceneManager.LoadScene(_scene);
        }
        else
        {
            print($"Please add scene name in {transform.parent.name}!");
        }
    }

    public bool IsInspectable()
    {
        return false;
    }

    public bool IsInteractable()
    {
        return true;
    }
}

