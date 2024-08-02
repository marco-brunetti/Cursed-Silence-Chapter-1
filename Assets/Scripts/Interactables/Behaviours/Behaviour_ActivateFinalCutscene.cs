using SnowHorse.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Behaviour_ActivateFinalCutscene : MonoBehaviour, IBehaviour
{
    [Header("Delays")]
    [SerializeField] private float _activationDelay = 2f;
    [SerializeField] private float _sceneUnloadDelay = 1.7f;

    [Header("GameObjects")]
    [SerializeField] private GameObject _finalCamera;
    [SerializeField] private GameObject _idleMonster;
    [SerializeField] private GameObject _jumpingMonster;

    [Header("Other")]
    [SerializeField] private Transform _targetRotation;

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
        yield return new WaitForSeconds(_activationDelay);

        Destroy(PlayerController.Instance.gameObject);

        _finalCamera.SetActive(true);
        _idleMonster.SetActive(false);
        _jumpingMonster.SetActive(true);

        _sceneActive = true;

        yield return new WaitForSeconds(_sceneUnloadDelay);

        _finalCamera.SetActive(false);
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
                _initialRotation = _finalCamera.transform.rotation;
                _initialRotationSet = true;
            }

            float percentage = Interpolation.Smoother(1.5f, ref _currentTime);

            _finalCamera.transform.rotation = Quaternion.Slerp(_initialRotation, _targetRotation.rotation, percentage);
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
