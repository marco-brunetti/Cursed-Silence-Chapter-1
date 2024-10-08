using UnityEngine;

namespace Player
{
    public interface IPlayerInput
    {
        //bool FlashLightInput { get; }
        Vector2 mouseMovementInput { get; }
        Vector2 UnsmoothedMouseInput { get; }
        float MouseScrollInput { get; }
        bool playerJumpInput { get; }
        Vector2 playerMovementInput { get; }
        Vector2 UnsmoothedPlayerMovementInput { get; }
        bool playerInteractInput { get; }
        bool playerRunInput { get; }
    }


    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        private MainInput _mainInput;
        public bool playerJumpInput { get; private set; }
        public bool playerRunInput { get; private set; }

        public bool playerInteractInput { get; private set; }

        //public bool FlashLightInput { get; private set; }
        public float MouseScrollInput { get; private set; }
        public Vector2 playerMovementInput { get; private set; }
        public Vector2 UnsmoothedPlayerMovementInput { get; private set; }
        public Vector2 mouseMovementInput { get; private set; }
        public Vector2 UnsmoothedMouseInput { get; private set; }

        //Variables for smoothing player move
        private float smoothTime = 0.1f;
        private Vector2 currentVelocity;

        //Variables for smoothing player rotate
        private Vector2 currentMouseVelocity;

        private void Awake()
        {
            //New input manager
            _mainInput = new MainInput();
            _mainInput.Player.Enable();
        }


        private void Update()
        {
            InputSystem();
            MouseScrollFix();

            SmoothPlayerMovement();
            SmoothPlayerRotate();
        }

        private void InputSystem()
        {
            playerJumpInput = _mainInput.Player.Jump.inProgress;
            playerRunInput = _mainInput.Player.Run.inProgress;
            playerInteractInput = _mainInput.Player.ItemPickup.triggered;
        }

        private void SmoothPlayerMovement()
        {
            UnsmoothedPlayerMovementInput = _mainInput.Player.Move.ReadValue<Vector2>();
            playerMovementInput = Vector2.SmoothDamp(playerMovementInput, UnsmoothedPlayerMovementInput,
                ref currentVelocity, smoothTime);
        }

        private void SmoothPlayerRotate()
        {
            UnsmoothedMouseInput = _mainInput.Player.Rotate.ReadValue<Vector2>();
            mouseMovementInput = Vector2.SmoothDamp(mouseMovementInput, UnsmoothedMouseInput, ref currentMouseVelocity,
                smoothTime);
        }

        private void MouseScrollFix()
        {
            MouseScrollInput = _mainInput.Player.Scroll.ReadValue<float>();
            MouseScrollInput =
                Mathf.Abs(MouseScrollInput) > 1
                    ? MouseScrollInput / 120f
                    : MouseScrollInput; //Fixes windows 120 scroll bug
        }
    }
}