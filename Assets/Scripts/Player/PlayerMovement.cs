using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private float _currentMoveSpeed = 0;
        private RaycastHit _hit;

        private float _decelerationTime = 0.5f;

        public void PlayerMove(PlayerData playerData, IPlayerInput input, Transform groundSpawnPoint, float currentVelocity)
        {
            if(input.playerMovementInput == Vector2.zero && Mathf.Approximately(currentVelocity, 0)) return;

            Vector3 forward = PlayerController.Instance.Camera.transform.TransformDirection(Vector3.forward);

            forward.y = 0;
            forward.Normalize();

            Vector3 right = PlayerController.Instance.Camera.transform.TransformDirection(Vector3.right);

            var targetSpeed = 0f;

            if (input.playerMovementInput != Vector2.zero)
            {
                var runSpeed = PlayerController.Instance.IsOutside ? playerData.RunSpeed : playerData.RunSpeed / 2;
                targetSpeed = input.playerRunInput ? runSpeed : playerData.WalkSpeed;
            }

            _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, targetSpeed, _decelerationTime);

            float curSpeedX = _currentMoveSpeed * input.playerMovementInput.y;
            float curSpeedY = _currentMoveSpeed * input.playerMovementInput.x;

            Vector3 moveDirection = Vector3.zero;

            float movementDirectionY = moveDirection.y;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection.y = movementDirectionY;

            Ray ray = new Ray
            {
                origin = groundSpawnPoint.position,
                direction = -groundSpawnPoint.transform.up
            };

            if (Physics.Raycast(ray, out _hit, 1))
                PlayerController.Instance.IsOutside = _hit.collider.CompareTag("Terrain");

            moveDirection.y += playerData.Gravity;
            PlayerController.Instance.Character.Move(moveDirection * Time.deltaTime);
        }
    }
}