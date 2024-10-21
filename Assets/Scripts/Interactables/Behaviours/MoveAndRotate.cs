using System.Collections;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class MoveAndRotate : MonoBehaviour, IBehaviour
    {
        #region Exposed Variables
        [FormerlySerializedAs("_object")] [SerializeField] private Transform @object;
        [FormerlySerializedAs("_movementTarget")]
        [Header("Movement")]
        [SerializeField] private Transform movementTarget;
        [FormerlySerializedAs("_movementDuration")] [SerializeField] private float movementDuration;
        [FormerlySerializedAs("_reverseMovement")] [SerializeField] private bool reverseMovement;

        [FormerlySerializedAs("_smoothStep")]
        [Header("Movement lerp type (Linear if unchecked)")]
        [SerializeField] private bool smoothStep;
        [FormerlySerializedAs("_smootherStep")] [SerializeField] private bool smootherStep;

        [FormerlySerializedAs("_rotationTarget")]
        [Header("Rotation")]
        [SerializeField] private Transform rotationTarget;
        [FormerlySerializedAs("_rotationDuration")] [SerializeField] private float rotationDuration;
        [FormerlySerializedAs("_reverseRotation")] [SerializeField] private bool reverseRotation;

        [FormerlySerializedAs("_smoothRotation")]
        [Header("Rotation lerp type (Linear if unchecked)")]
        [SerializeField] private bool smoothRotation;
        [FormerlySerializedAs("_smootherRotation")] [SerializeField] private bool smootherRotation;

        [FormerlySerializedAs("_deactivateWhenDone")]
        [Header("Action after finished (Deactivate recomended)")]
        [SerializeField] private bool deactivateWhenDone = true;

        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction;
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
                if (movementTarget != null)
                {
                    _movementStartPosition = @object.position;
                    _move = true;
                    StartCoroutine(ManageMovementTime(movementDuration));
                }

                if(rotationTarget != null)
                {
                    _rotationStartPosition = @object.rotation.eulerAngles;
                    _rotate = true;
                    StartCoroutine(ManageRotationTime(rotationDuration));
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

            if (smoothStep)
                percentage = Interpolation.Smooth(movementDuration, ref _tMovement, reverseMovement);
            else if (smootherStep)
                percentage = Interpolation.Smoother(movementDuration, ref _tMovement, reverseMovement);
            else
                percentage = Interpolation.Linear(movementDuration, ref _tMovement, reverseMovement);

            @object.position = Vector3.Lerp(_movementStartPosition, movementTarget.position, percentage);
        }

        private void ManageRotation()
        {
            float percentage;

            if (smoothStep)
                percentage = Interpolation.Smooth(movementDuration, ref _tRotation, reverseRotation);
            else if (smootherStep)
                percentage = Interpolation.Smoother(movementDuration, ref _tRotation, reverseRotation);
            else
                percentage = Interpolation.Linear(movementDuration, ref _tRotation, reverseRotation);

            @object.rotation = Quaternion.Lerp(Quaternion.Euler(_rotationStartPosition), rotationTarget.rotation, percentage);
        }
        #endregion

        #region Duration Managers
        IEnumerator ManageMovementTime(float movementDuration)
        {
            yield return new WaitForSeconds(movementDuration);
            _move = false;
            if(deactivateWhenDone)
                movementTarget = null;
        }

        IEnumerator ManageRotationTime(float rotateDuration)
        {
            yield return new WaitForSeconds(rotateDuration);
            _rotate = false;
            if (deactivateWhenDone)
                rotationTarget = null;
        }

        #endregion
        public bool IsInteractable()
        {
            return onInteraction;
        }

        public bool IsInspectable()
        {
            return onInspection;
        }
    }
}