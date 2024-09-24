using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class LevelLoader : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_scene")] [SerializeField] private string scene;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(scene != "")
            {
                SceneManager.LoadScene(scene);
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
}

