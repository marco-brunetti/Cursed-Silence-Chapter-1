using UnityEngine;

namespace SnowHorse.Components
{
    public class FollowObjectPosAndRot : MonoBehaviour
    {
        [Header("Follow Position")]
        [SerializeField] private bool _followPosition;
        [SerializeField] private Transform _followObject;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private float _smoothTime;
        private Vector3 currentVelocity;

        [Header("Follow Rotation")]
        [SerializeField] private bool _followRotation;
        [SerializeField] Transform _rotationTargetX, _rotationTargetY, _rotationTargetZ;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private float rotationSpeed;
        private Vector3 rot;

        // Update is called once per frame
        void Update()
        {
            FollowPosition();
            FollowRotation();
        }

        private void FollowPosition()
        {
            if (_followPosition)
            {
                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(_followObject.transform.position.x + positionOffset.x,
                _followObject.transform.position.y + positionOffset.y, _followObject.transform.position.z + positionOffset.z), ref currentVelocity, _smoothTime);
            }
        }
        private void FollowRotation()
        {
            if (_followRotation)
            {
                //gets rotation
                rot.x = _rotationTargetX.rotation.eulerAngles.x + rotationOffset.x;
                rot.y = _rotationTargetY.rotation.eulerAngles.y + rotationOffset.y;
                rot.z = _rotationTargetZ.rotation.eulerAngles.z + rotationOffset.z;

                //Rotates object
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot),
                        rotationSpeed * Time.deltaTime);
            }
        }
    }
}