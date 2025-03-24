using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    // Assign your pause menu panel in the inspector
    public GameObject pauseMenuUI;

    [Header("Input Action")]
    // Create an InputAction for toggling the menu. You can either set this up via code or assign it in the inspector.
    // If not assigned, it will be created with the Escape key binding.
    public InputAction toggleMenuAction = new InputAction(binding: "<Keyboard>/escape");

    private bool isPaused = false;

    private void OnEnable()
    {
        // Enable the action and subscribe to its performed event
        toggleMenuAction.Enable();
        toggleMenuAction.performed += OnToggleMenu;
    }

    private void OnDisable()
    {
        // Unsubscribe and disable the action when the object is disabled
        toggleMenuAction.performed -= OnToggleMenu;
        toggleMenuAction.Disable();
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    // Toggles the pause menu visibility and game pause state
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f; // Pause or resume the game
    }

    // Loads the main menu scene (ensure your main menu scene is named "MainMenu" or adjust accordingly)
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Resume game time before loading a new scene
        SceneManager.LoadScene("MainMenu");
    }

    // Quits the game application
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
