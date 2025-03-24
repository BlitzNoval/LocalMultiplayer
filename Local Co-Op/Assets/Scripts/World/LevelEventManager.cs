// Title: Building a custom event system 
// Author: Game Dev Guide
// Date: 22 March  2025
// Availability: https://youtu.be/gx0Lt4tCDE0?si=RcVrtDWzNLZeFk5D

// Title: Debugging 
// Author: ChatGPT
// Date: 22 March  2025
// Used to debug errors which were annoying , essentially when the player was entering it would use double the percentage becuase of the 
// 3 Trigger Colliders on the players , got around this but it kept using double the charge so ChatGPT just helped me debug it


using UnityEngine;
using System.Collections;
using System;

public class LevelEventManager : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform[] doorTiles;
    public float doorMoveDistance = 1f;
    public float playerDetectedDoorMoveDistance = 0.5f;
    public float doorAnimationDuration = 1f;

    [Header("Doorway Triggers")]
    public GameObject[] doorwayTriggerObjects;

    [Header("Test Timer Settings")]
    public float testTimerDuration = 45f;
    private float testTimer = 0f;

    public event Action OnActivateEvent;
    public event Action OnResetEvent;

    private DoorwayTrigger[] doorwayTriggers;
    private Vector3[] doorOpenPositions;
    private bool doorsClosed = false;

    void Start()
    {
        doorwayTriggers = new DoorwayTrigger[doorwayTriggerObjects.Length];

        if (doorwayTriggerObjects.Length != doorTiles.Length)
        {
            Debug.LogWarning("The number of doorway trigger objects does not match the number of door tiles. " +
                             "Only the first " + Mathf.Min(doorwayTriggerObjects.Length, doorTiles.Length) + " pairs will be used.");
        }

        int pairCount = Mathf.Min(doorTiles.Length, doorwayTriggerObjects.Length);
        doorOpenPositions = new Vector3[doorTiles.Length];

        for (int i = 0; i < doorTiles.Length; i++)
        {
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
        if (!doorsClosed)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testTimerDuration)
            {
                OnEvent60Seconds();
            }
        }
    }
    
    // Add the GetTimerProgress() method so that other scripts can read the timer progress.
    public float GetTimerProgress()
    {
        return Mathf.Clamp01(testTimer / testTimerDuration);
    }

    public void OnEvent60Seconds()
    {
        if (!doorsClosed)
        {
            StartCoroutine(CloseDoors());
            if (OnActivateEvent != null)
            {
                OnActivateEvent();
            }
        }
    }

    private IEnumerator CloseDoors()
    {
        Vector3[] initialPositions = new Vector3[doorTiles.Length];
        float[] elapsedTimes = new float[doorTiles.Length];
        bool[] doorsFinished = new bool[doorTiles.Length];
        bool[] wasPaused = new bool[doorTiles.Length];

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
                if (i < doorwayTriggers.Length && doorwayTriggers[i] != null && doorwayTriggers[i].IsPlayerInDoorway())
                {
                    if (!wasPaused[i])
                    {
                        int playerCount = doorwayTriggers[i].GetPlayerCount();
                        Debug.Log("Door " + i + " paused: " + playerCount + " player(s) still in the doorway.");
                        wasPaused[i] = true;
                    }
                    allFinished = false;
                    continue;
                }
                else
                {
                    if (wasPaused[i])
                    {
                        Debug.Log("Door " + i + " resumed closing: all players left the doorway.");
                        wasPaused[i] = false;
                    }
                }

                if (!doorsFinished[i])
                {
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

    public void ResetTestEvent()
    {
        testTimer = 0f;
        for (int i = 0; i < doorTiles.Length; i++)
        {
            doorTiles[i].position = doorOpenPositions[i];
        }
        doorsClosed = false;

        if (OnResetEvent != null)
        {
            OnResetEvent();
        }
    }
}
