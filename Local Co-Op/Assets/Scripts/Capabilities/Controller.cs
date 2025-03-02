using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public InputController input; // Stores the player's input configuration
    private PlayerInput _playerInput; // Unity’s PlayerInput component

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>(); // Get the PlayerInput component

        // 🔹 Ensure each player gets a **new unique instance** of PlayerController
        input = Instantiate(ScriptableObject.CreateInstance<PlayerController>());

        // 🔹 Initialize the Input System for this specific player
        ((PlayerController)input).Initialize(_playerInput);
    }
}
