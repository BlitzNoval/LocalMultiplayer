using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;

    [Header("Cinemachine")]
    public CinemachineTargetGroup targetGroup;

    private List<PlayerInput> spawnedPlayers = new List<PlayerInput>();

    void Start()
    {
        if (playerPrefabs.Length != spawnPoints.Length)
        {
            Debug.LogError("Number of player prefabs and spawn points must match.");
            return;
        }
        
        SpawnAllPlayers();
    }

    public void SpawnAllPlayers()
    {
        foreach (var player in spawnedPlayers)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
        spawnedPlayers.Clear();

        if (targetGroup != null)
        {
            targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        }
        else
        {
            Debug.LogError("CinemachineTargetGroup is not assigned in the Inspector.");
        }

        int playerCount = 0;

        foreach (InputDevice device in InputSystem.devices)
        {
            if (device is Keyboard || device is Gamepad)
            {
                if (playerCount >= playerPrefabs.Length)
                {
                    Debug.LogWarning("More devices than prefabs. Stopping spawn.");
                    break;
                }

                PlayerInput newPlayer = PlayerInput.Instantiate(
                    playerPrefabs[playerCount],
                    playerIndex: playerCount,
                    pairWithDevice: device
                );

                if (newPlayer != null)
                {
                    newPlayer.transform.position = spawnPoints[playerCount].position;
                    spawnedPlayers.Add(newPlayer);

                    if (targetGroup != null)
                    {
                        targetGroup.AddMember(newPlayer.transform, 1f, 0f);
                    }
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
