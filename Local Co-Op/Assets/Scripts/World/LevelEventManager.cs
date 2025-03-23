using UnityEngine;
using System.Collections;

public class LevelEventManager : MonoBehaviour
{
    [Header("Door Settings")]
    // Assign the door tile transforms in the Inspector
    public Transform[] doorTiles;
    // The distance the door will move upward when closing
    public float doorMoveDistance = 1f;
    // Duration over which the door animation will occur
    public float doorAnimationDuration = 1f;

    [Header("Test Timer Settings")]
    // Test timer duration (set to 60 seconds for testing)
    public float testTimerDuration = 60f;
    private float testTimer = 0f;

    // Keep track of the door open positions (default positions)
    private Vector3[] doorOpenPositions;
    private bool doorsClosed = false;

    void Start()
    {
        // Record the original (open) positions for each door tile
        doorOpenPositions = new Vector3[doorTiles.Length];
        for (int i = 0; i < doorTiles.Length; i++)
        {
            doorOpenPositions[i] = doorTiles[i].position;
        }
    }

    void Update()
    {
        // For testing: increment timer and trigger the event at testTimerDuration seconds.
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
    /// It will trigger the door closing animation.
    /// </summary>
    public void OnEvent60Seconds()
    {
        if (!doorsClosed)
        {
            StartCoroutine(CloseDoors());
            // You can also trigger teleporter logic here if needed.
        }
    }

    /// <summary>
    /// Coroutine that smoothly moves the door tiles upward to simulate closing.
    /// </summary>
    private IEnumerator CloseDoors()
    {
        float elapsedTime = 0f;
        // Record the current positions of each door tile
        Vector3[] initialPositions = new Vector3[doorTiles.Length];
        Vector3[] targetPositions = new Vector3[doorTiles.Length];

        for (int i = 0; i < doorTiles.Length; i++)
        {
            initialPositions[i] = doorTiles[i].position;
            // Calculate the target position: moving up by doorMoveDistance
            targetPositions[i] = initialPositions[i] + new Vector3(0, doorMoveDistance, 0);
        }

        // Animate the door movement over doorAnimationDuration seconds
        while (elapsedTime < doorAnimationDuration)
        {
            for (int i = 0; i < doorTiles.Length; i++)
            {
                doorTiles[i].position = Vector3.Lerp(initialPositions[i], targetPositions[i], elapsedTime / doorAnimationDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ensure doors reach the final position
        for (int i = 0; i < doorTiles.Length; i++)
        {
            doorTiles[i].position = targetPositions[i];
        }
        doorsClosed = true;
    }

    /// <summary>
    /// Resets the test timer and re-opens the doors.
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
    }
}
