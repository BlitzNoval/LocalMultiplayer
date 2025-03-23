using UnityEngine;
using System.Collections;
using System;

public class LevelEventManager : MonoBehaviour
{
    [Header("Door Settings")]
    // Assign the door tile transforms in the Inspector
    public Transform[] doorTiles;
    // Default distance the door will move upward when closing (if no player is detected)
    public float doorMoveDistance = 1f;
    // Distance the door will move when a player is detected in its doorway
    public float playerDetectedDoorMoveDistance = 0.5f;
    // Duration over which the door animation will occur
    public float doorAnimationDuration = 1f;

    [Header("Doorway Triggers")]
    // Assign separate GameObjects that contain the DoorwayTrigger components (with 2D BoxColliders checking for the "Player" tag)
    public GameObject[] doorwayTriggerObjects;

    [Header("Test Timer Settings")]
    // Test timer duration (set to 60 seconds for testing)
    public float testTimerDuration = 60f;
    private float testTimer = 0f;

    // Event that triggers when the 60-second mark is reached
    public event Action OnActivateEvent;
    // Event that triggers when the level is reset
    public event Action OnResetEvent;

    // Array to store DoorwayTrigger components retrieved from doorwayTriggerObjects
    private DoorwayTrigger[] doorwayTriggers;
    // Keep track of the door open positions (default positions)
    private Vector3[] doorOpenPositions;
    private bool doorsClosed = false;

    void Start()
{
    // Initialize the doorwayTriggers array based on the assigned GameObjects
    doorwayTriggers = new DoorwayTrigger[doorwayTriggerObjects.Length];

    // Warn if the number of doorway trigger objects does not match the number of door tiles.
    if (doorwayTriggerObjects.Length != doorTiles.Length)
    {
        Debug.LogWarning("The number of doorway trigger objects does not match the number of door tiles. " +
                         "Only the first " + Mathf.Min(doorwayTriggerObjects.Length, doorTiles.Length) + " pairs will be used.");
    }

    // Determine how many pairs to initialize (use the smaller count)
    int pairCount = Mathf.Min(doorTiles.Length, doorwayTriggerObjects.Length);

    // Retrieve DoorwayTrigger components and store initial door positions for the matching pairs
    doorOpenPositions = new Vector3[doorTiles.Length];
    for (int i = 0; i < doorTiles.Length; i++)
    {
        // Store the initial position of each door
        doorOpenPositions[i] = doorTiles[i].position;
    }

    for (int i = 0; i < pairCount; i++)
    {
        if (doorwayTriggerObjects[i] != null)
        {
            doorwayTriggers[i] = doorwayTriggerObjects[i].GetComponent<DoorwayTrigger>();
            if (doorwayTriggers[i] == null)
            {
                Debug.LogError("DoorwayTrigger component not found on " + doorwayTriggerObjects[i].name);
            }
        }
        else
        {
            Debug.LogError("Doorway trigger object " + i + " is not assigned.");
        }
    }
}


    void Update()
    {
        // For testing: increment timer and trigger the event at testTimerDuration seconds
        if (!doorsClosed)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testTimerDuration)
            {
                OnEvent60Seconds();
            }
        }
    }

    /// <summary>
    /// Call this method when the level reaches 60 seconds.
    /// It will trigger the door closing animation and notify subscribers.
    /// </summary>
    public void OnEvent60Seconds()
    {
        if (!doorsClosed)
        {
            StartCoroutine(CloseDoors());
            // Notify teleporters and other listeners that the activation time has been reached
            if (OnActivateEvent != null)
            {
                OnActivateEvent();
            }
        }
    }

    /// <summary>
    /// Coroutine that smoothly moves each door tile upward to simulate closing.
    /// Each door will move to a target position that depends on whether a player is detected in its doorway.
    /// </summary>
private IEnumerator CloseDoors()
{
    Vector3[] initialPositions = new Vector3[doorTiles.Length];
    float[] elapsedTimes = new float[doorTiles.Length];
    bool[] doorsFinished = new bool[doorTiles.Length];
    // Track the previous paused state for each door so we only log on state changes.
    bool[] wasPaused = new bool[doorTiles.Length];

    // Initialize positions, timers, and paused states for each door
    for (int i = 0; i < doorTiles.Length; i++)
    {
        initialPositions[i] = doorTiles[i].position;
        elapsedTimes[i] = 0f;
        doorsFinished[i] = false;
        wasPaused[i] = false;
    }

    while (true)
    {
        bool allFinished = true;
        for (int i = 0; i < doorTiles.Length; i++)
        {
            // Check if the door has an associated trigger and if the player is inside.
            if (i < doorwayTriggers.Length && doorwayTriggers[i] != null && doorwayTriggers[i].IsPlayerInDoorway())
            {
                // Only log once when the door becomes paused.
                if (!wasPaused[i])
                {
                    Debug.Log("Door " + i + " paused: player is still in the doorway.");
                    wasPaused[i] = true;
                }
                allFinished = false;
                continue; // Skip updating this door until the player leaves.
            }
            else
            {
                // If the door was paused in the previous frame, log that it has resumed.
                if (wasPaused[i])
                {
                    Debug.Log("Door " + i + " resumed closing: player left the doorway.");
                    wasPaused[i] = false;
                }
            }

            if (!doorsFinished[i])
            {
                // Only update the door's timer if no player is in its doorway
                elapsedTimes[i] += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTimes[i] / doorAnimationDuration);
                Vector3 targetPosition = initialPositions[i] + new Vector3(0, doorMoveDistance, 0);
                doorTiles[i].position = Vector3.Lerp(initialPositions[i], targetPosition, t);
                if (t >= 1f)
                {
                    doorsFinished[i] = true;
                }
            }
            if (!doorsFinished[i])
            {
                allFinished = false;
            }
        }
        if (allFinished)
        {
            break;
        }
        yield return null;
    }
    doorsClosed = true;
}



    /// <summary>
    /// Returns the progress of the test timer as a value between 0 and 1.
    /// </summary>
    public float GetTimerProgress()
    {
        return Mathf.Clamp01(testTimer / testTimerDuration);
    }

    /// <summary>
    /// Resets the test timer, re-opens the doors, and notifies subscribers.
    /// This method can be attached to a UI Button.
    /// </summary>
    public void ResetTestEvent()
    {
        // Reset timer
        testTimer = 0f;
        // Reset door positions to their original (open) state
        for (int i = 0; i < doorTiles.Length; i++)
        {
            doorTiles[i].position = doorOpenPositions[i];
        }
        doorsClosed = false;

        // Notify teleporters and other listeners to reset
        if (OnResetEvent != null)
        {
            OnResetEvent();
        }
    }
}
