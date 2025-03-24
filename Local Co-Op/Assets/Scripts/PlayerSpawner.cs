using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public Transform[] spawnPoints; // Assign spawn points in the inspector

    private int playerCount = 0; // Track spawned players 

    void Start()
    {
        if (playerInputManager == null)
        {
            playerInputManager = FindObjectOfType<PlayerInputManager>();
        }

        SpawnAllPlayers();
    }

    void SpawnAllPlayers()
    {
        playerCount = 0;

        foreach (InputDevice device in InputSystem.devices)
        {
            // Ensure the device is valid (Keyboard or Gamepad)
            if (device == Keyboard.current || device == Gamepad.current)
            {
                // Spawn the player
                PlayerInput newPlayer = playerInputManager.JoinPlayer(playerCount, -1, null, device);

                // Set spawn position if available
                if (spawnPoints.Length > playerCount)
                {
                    newPlayer.transform.position = spawnPoints[playerCount].position;
                }

                playerCount++;

                // Stop if we've reached the max players
                if (playerCount >= playerInputManager.maxPlayerCount)
                    break;
            }
        }
    }
}
