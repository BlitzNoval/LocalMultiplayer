using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs; // Assign different prefabs for each spawn point in the Inspector
    public Transform[] spawnPoints;    // Assign spawn points in the Inspector

    private int playerCount = 0;       // Track spawned players

    void Start()
    {
        if (playerPrefabs.Length != spawnPoints.Length)
        {
            Debug.LogError("Number of player prefabs and spawn points must match.");
            return;
        }
        SpawnAllPlayers();
    }

    void SpawnAllPlayers()
    {
        playerCount = 0;

        foreach (InputDevice device in InputSystem.devices)
        {
            if (device is Keyboard || device is Gamepad)
            {
                if (playerCount >= playerPrefabs.Length)
                {
                    Debug.LogWarning("More devices than prefabs. Stopping spawn.");
                    break;
                }

                // Instantiate player with specific prefab and device
                PlayerInput newPlayer = PlayerInput.Instantiate(
                    playerPrefabs[playerCount],
                    playerIndex: playerCount,
                    pairWithDevice: device
                );

                if (newPlayer != null)
                {
                    newPlayer.transform.position = spawnPoints[playerCount].position;
                }
                else
                {
                    Debug.LogError("Failed to instantiate player for device: " + device.name);
                }

                playerCount++;
            }
        }
    }
}