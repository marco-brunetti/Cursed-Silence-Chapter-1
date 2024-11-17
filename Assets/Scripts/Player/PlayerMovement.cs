using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private readonly float _maxSpeedChange = 10f;
        private float _currentMoveSpeed = 0;

        private PlayerController _playerController;

        private void Start()
        {
            _playerController = PlayerController.Instance;
        }

        public void PlayerMove(PlayerData playerData, IPlayerInput input, float currentVelocity)
        {
            if(input.playerMovementInput == Vector2.zero && Mathf.Approximately(currentVelocity, 0)) return;

            Vector3 forward = _playerController.Camera.transform.TransformDirection(Vector3.forward);

            forward.y = 0;
            forward.Normalize();

            Vector3 right = _playerController.Camera.transform.TransformDirection(Vector3.right);

            var targetSpeed = 0f;

            if (input.playerMovementInput != Vector2.zero)
            {
                var runSpeed = _playerController.IsOutside ? playerData.RunSpeed : playerData.RunSpeed / 2;
                targetSpeed = input.playerRunInput ? runSpeed : playerData.WalkSpeed;
            }

            _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, targetSpeed, _maxSpeedChange);

            float curSpeedX = _currentMoveSpeed * input.playerMovementInput.y;
            float curSpeedY = _currentMoveSpeed * input.playerMovementInput.x;

            Vector3 moveDirection = Vector3.zero;

            float movementDirectionY = moveDirection.y;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection.y = movementDirectionY;

            moveDirection.y += playerData.Gravity;
            _playerController.Character.Move(moveDirection * Time.deltaTime);
        }
    }
}