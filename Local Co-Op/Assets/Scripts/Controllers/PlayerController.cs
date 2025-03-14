using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    private PlayerInputActions _inputActions;
    private bool _isJumping;

    public void Initialize(PlayerInput playerInput)
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
        }

        // 🔹 Bind this player's unique input actions
        _inputActions.Gameplay.Enable();
        playerInput.actions = _inputActions.asset;

        _inputActions.Gameplay.Jump.started += JumpStarted;
        _inputActions.Gameplay.Jump.canceled += JumpCanceled;
    }

    private void OnDisable()
{
    if (_inputActions != null)  // Check before using this function
    {
        _inputActions.Gameplay.Disable();
        _inputActions.Gameplay.Jump.started -= JumpStarted;
        _inputActions.Gameplay.Jump.canceled -= JumpCanceled;
        _inputActions = null;
    }
}


    private void JumpStarted(InputAction.CallbackContext obj)
    {
        _isJumping = true;
    }

    private void JumpCanceled(InputAction.CallbackContext obj)
    {
        _isJumping = false;
    }

    public override bool RetrieveJumpInput()
    {
        return _isJumping;
    }

    public override float RetrieveMoveInput()
    {
        return _inputActions.Gameplay.Move.ReadValue<Vector2>().x;
    }
}
