// Title: The ULTIMATE 2D Character CONTROLLER in UNITY
// Author: Shinjingi
// Date: 01 March  2025
// Availability: https://www.youtube.com/watch?v=lcw6nuc2uaU

using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public InputController input; // Storing the player's input here pls
    private PlayerInput _playerInput; 

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>(); // Get the PlayerInput here pls

        
        input = Instantiate(ScriptableObject.CreateInstance<PlayerController>());

        
        ((PlayerController)input).Initialize(_playerInput);
    }
}
