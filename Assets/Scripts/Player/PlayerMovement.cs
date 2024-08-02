using UnityEngine;
using UnityEngine.Windows;

public interface IPlayerMovement
{
    void PlayerMove(PlayerData playerData, IPlayerInput playerInput);
}

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    public void PlayerMove(PlayerData playerData, IPlayerInput input)
    {
        Vector3 moveDirection = Vector3.zero;

        if (input.playerMovementInput != Vector2.zero)
        {
            Vector3 forward = playerData.Camera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward.Normalize();

            Vector3 right = playerData.Camera.transform.TransformDirection(Vector3.right);

            float curSpeedX = (input.playerRunInput ? playerData.RunSpeed : playerData.WalkSpeed) * input.playerMovementInput.y;
            float curSpeedY = (input.playerRunInput ? playerData.RunSpeed : playerData.WalkSpeed) * input.playerMovementInput.x;
            float movementDirectionY = moveDirection.y;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection.y = movementDirectionY;
        }

        moveDirection.y += playerData.Gravity;
        playerData.Character.Move(moveDirection * Time.deltaTime);
    }
}