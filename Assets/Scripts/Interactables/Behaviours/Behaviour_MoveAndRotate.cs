using System.Collections;
using SnowHorse.Utils;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_MoveAndRotate : MonoBehaviour, IBehaviour
    {
        #region Exposed Variables
        [SerializeField] private Transform _object;
        [Header("Movement")]
        [SerializeField] private Transform _movementTarget;
        [SerializeField] private float _movementDuration;
        [SerializeField] private bool _reverseMovement;

        [Header("Movement lerp type (Linear if unchecked)")]
        [SerializeField] private bool _smoothStep;
        [SerializeField] private bool _smootherStep;

        [Header("Rotation")]
        [SerializeField] private Transform _rotationTarget;
        [SerializeField] private float _rotationDuration;
        [SerializeField] private bool _reverseRotation;

        [Header("Rotation lerp type (Linear if unchecked)")]
        [SerializeField] private bool _smoothRotation;
        [SerializeField] private bool _smootherRotation;

        [Header("Action after finished (Deactivate recomended)")]
        [SerializeField] private bool _deactivateWhenDone = true;

        [SerializeField] private bool _onInteraction;
        [SerializeField] private bool onInspection;

        #endregion

        #region private variables
        private bool _move;
        private bool _rotate;

        private float _tMovement;
        private Vector3 _movementStartPosition;

        private float _tRotation;
        private Vector3 _rotationStartPosition;
        #endregion

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(isInteracting || isInspecting)
            {
                if (_movementTarget != null)
                {
                    _movementStartPosition = _object.position;
                    _move = true;
                    StartCoroutine(ManageMovementTime(_movementDuration));
                }

                if(_rotationTarget != null)
                {
                    _rotationStartPosition = _object.rotation.eulerAngles;
                    _rotate = true;
                    StartCoroutine(ManageRotationTime(_rotationDuration));
                }
            }
        }

        private void Update()
        {
            if(_move)
            {
                ManageMovement();
            }
            if(_rotate)
            {
                ManageRotation();
            }
        }

        #region Lerp Managers
        private void ManageMovement()
        {
            float percentage;

            if (_smoothStep)
                percentage = Interpolation.Smooth(_movementDuration, ref _tMovement, _reverseMovement);
            else if (_smootherStep)
                percentage = Interpolation.Smoother(_movementDuration, ref _tMovement, _reverseMovement);
            else
                percentage = Interpolation.Linear(_movementDuration, ref _tMovement, _reverseMovement);

            _object.position = Vector3.Lerp(_movementStartPosition, _movementTarget.position, percentage);
        }

        private void ManageRotation()
        {
            float percentage;

            if (_smoothStep)
                percentage = Interpolation.Smooth(_movementDuration, ref _tRotation, _reverseRotation);
            else if (_smootherStep)
                percentage = Interpolation.Smoother(_movementDuration, ref _tRotation, _reverseRotation);
            else
                percentage = Interpolation.Linear(_movementDuration, ref _tRotation, _reverseRotation);

            _object.rotation = Quaternion.Lerp(Quaternion.Euler(_rotationStartPosition), _rotationTarget.rotation, percentage);
        }
        #endregion

        #region Duration Managers
        IEnumerator ManageMovementTime(float movementDuration)
        {
            yield return new WaitForSeconds(movementDuration);
            _move = false;
            if(_deactivateWhenDone)
                _movementTarget = null;
        }

        IEnumerator ManageRotationTime(float rotateDuration)
        {
            yield return new WaitForSeconds(rotateDuration);
            _rotate = false;
            if (_deactivateWhenDone)
                _rotationTarget = null;
        }

        #endregion
        public bool IsInteractable()
        {
            return _onInteraction;
        }

        public bool IsInspectable()
        {
            return onInspection;
        }
    }
}