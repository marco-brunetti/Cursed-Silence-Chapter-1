using System.Collections;
using Player;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class ActivateFinalCutscene : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_activationDelay")]
        [Header("Delays")]
        [SerializeField] private float activationDelay = 2f;
        [FormerlySerializedAs("_sceneUnloadDelay")] [SerializeField] private float sceneUnloadDelay = 1.7f;

        [FormerlySerializedAs("_finalCamera")]
        [Header("GameObjects")]
        [SerializeField] private GameObject finalCamera;
        [FormerlySerializedAs("_idleMonster")] [SerializeField] private GameObject idleMonster;
        [FormerlySerializedAs("_jumpingMonster")] [SerializeField] private GameObject jumpingMonster;

        [FormerlySerializedAs("_targetRotation")]
        [Header("Other")]
        [SerializeField] private Transform targetRotation;

        private bool _sceneActive;
        bool _initialRotationSet;

        private float _currentTime;

        private Quaternion _initialRotation;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            StartCoroutine(FinalScene());
        }

        private IEnumerator FinalScene()
        {
            yield return new WaitForSeconds(activationDelay);

            Destroy(PlayerController.Instance.gameObject);

            finalCamera.SetActive(true);
            idleMonster.SetActive(false);
            jumpingMonster.SetActive(true);

            _sceneActive = true;

            yield return new WaitForSeconds(sceneUnloadDelay);

            finalCamera.SetActive(false);
            UIManager.Instance.CanvasControl.OnSceneChanged(false);

            SceneManager.LoadScene("Level_House");   
        }


        private void Update()
        {
            ManageCameraAnimation();
        }

        private void ManageCameraAnimation()
        {
            if (_sceneActive)
            {
                if (!_initialRotationSet)
                {
                    _initialRotation = finalCamera.transform.rotation;
                    _initialRotationSet = true;
                }

                float percentage = Interpolation.Smoother(1.5f, ref _currentTime);

                finalCamera.transform.rotation = Quaternion.Slerp(_initialRotation, targetRotation.rotation, percentage);
            }
        }

        public bool IsInteractable()
        {
            return true;
        }

        public bool IsInspectable()
        {
            return false;
        }
    }
}
