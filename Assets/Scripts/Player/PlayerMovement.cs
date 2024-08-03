using UnityEngine;

public interface IPlayerMovement
{
    void PlayerMove(PlayerData playerData, IPlayerInput playerInput, Transform groundSpawnPoint);
}

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    private float _currentMoveSpeed = 0;
    private RaycastHit _hit;

    public void PlayerMove(PlayerData playerData, IPlayerInput input, Transform groundSpawnPoint)
    {
        Vector3 moveDirection = Vector3.zero;

        if (input.playerMovementInput != Vector2.zero)
        {
            Vector3 forward = PlayerController.Instance.Camera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward.Normalize();

            Vector3 right = PlayerController.Instance.Camera.transform.TransformDirection(Vector3.right);

            var runSpeed = PlayerController.Instance.IsOutside ? playerData.RunSpeed : playerData.RunSpeed / 2;

            _currentMoveSpeed = input.playerRunInput ? Mathf.MoveTowards(_currentMoveSpeed, runSpeed, 0.3f) :
                Mathf.MoveTowards(_currentMoveSpeed, playerData.WalkSpeed, 0.3f);

            float curSpeedX = _currentMoveSpeed * input.playerMovementInput.y;
            float curSpeedY = _currentMoveSpeed * input.playerMovementInput.x;
            float movementDirectionY = moveDirection.y;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection.y = movementDirectionY;

            Ray ray = new Ray
            {
                origin = groundSpawnPoint.position,
                direction = -groundSpawnPoint.transform.up
            };

            if(Physics.Raycast(ray, out _hit, 1)) PlayerController.Instance.IsOutside = _hit.collider.CompareTag("Terrain");
        }

        moveDirection.y += playerData.Gravity;
        PlayerController.Instance.Character.Move(moveDirection * Time.deltaTime);
    }
}