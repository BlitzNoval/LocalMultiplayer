// Title: Input Device Checker
// Author: ChatGPT
// Date: 09 March  2025
// Using to check for inptus on main menu 

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class InputDeviceChecker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;
    [SerializeField] private Image player1Panel;
    [SerializeField] private Image player2Panel;
    [SerializeField] private Button startGameButton;

    [Header("Connection Settings")]
    [SerializeField] private Color connectedColor = Color.green;
    [SerializeField] private Color disconnectedColor = Color.red;
    [SerializeField] private float checkInterval = 5f; // Check every 5 seconds
    
    private bool keyboardConnected = false;
    private bool gamepadConnected = false;
    private float timeSinceLastCheck = 0f;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnInputDeviceChange;
        CheckConnectedDevices();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnInputDeviceChange;
    }

    private void Start()
    {
        UpdateConnectionUI();
    }

    private void Update()
    {
        // Periodically check for connected devices
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            CheckConnectedDevices();
            UpdateConnectionUI();
            timeSinceLastCheck = 0f;
        }
    }

    private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // When devices are added or removed, update our connection status immediately
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            CheckConnectedDevices();
            UpdateConnectionUI();
            timeSinceLastCheck = 0f; // Reset timer
        }
    }

    private void CheckConnectedDevices()
    {
        // Check for keyboard
        var keyboards = InputSystem.devices.Where(d => d is Keyboard).ToList();
        keyboardConnected = keyboards.Count > 0;

        // Check for gamepads
        var gamepads = InputSystem.devices.Where(d => d is Gamepad).ToList();
        gamepadConnected = gamepads.Count > 0;
        
        Debug.Log($"Input devices found: {keyboards.Count} keyboards, {gamepads.Count} gamepads");
    }

    private void UpdateConnectionUI()
    {
        // Update Player 1 text and panel (Keyboard)
        if (player1Text != null)
        {
            player1Text.text = keyboardConnected ? "Player 1\nConnected" : "Player 1\nDisconnected";
        }
        if (player1Panel != null)
        {
            player1Panel.color = keyboardConnected ? connectedColor : disconnectedColor;
        }

        // Update Player 2 text and panel (Gamepad/Controller)
        if (player2Text != null)
        {
            player2Text.text = gamepadConnected ? "Player 2\nConnected" : "Player 2\nDisconnected";
        }
        if (player2Panel != null)
        {
            player2Panel.color = gamepadConnected ? connectedColor : disconnectedColor;
        }

        // Update start button interactability
        if (startGameButton != null)
        {
            bool bothPlayersConnected = keyboardConnected && gamepadConnected;
            startGameButton.interactable = bothPlayersConnected;
            
            // Optional: Change button visual to indicate it's disabled
            Image startButtonImage = startGameButton.GetComponent<Image>();
            if (startButtonImage != null)
            {
                Color color = startButtonImage.color;
                color.a = bothPlayersConnected ? 1f : 0.5f;
                startButtonImage.color = color;
            }
        }
    }

    // Public method to manually trigger a check and update
    public void RefreshDeviceStatus()
    {
        CheckConnectedDevices();
        UpdateConnectionUI();
        timeSinceLastCheck = 0f; // Reset timer
    }
}