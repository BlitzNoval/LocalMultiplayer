using UnityEngine;

public class DoorwayTrigger : MonoBehaviour
{
    // Counter to track how many players are in the doorway
    private int playersInDoorway = 0;

    // Called when another collider enters the trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter: " + other.name + " with tag: " + other.tag);
        if(other.CompareTag("Untagged"))
        {
            playersInDoorway++;
            Debug.Log("Untagged detected in doorway: " + gameObject.name + ", count: " + playersInDoorway);
        }
    }

    // Called when another collider exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exit: " + other.name + " with tag: " + other.tag);
        if(other.CompareTag("Untagged"))
        {
            playersInDoorway = Mathf.Max(0, playersInDoorway - 1); // Ensure we don't go below 0
            Debug.Log("Untagged left doorway: " + gameObject.name + ", count: " + playersInDoorway);
        }
    }

    // This method returns whether any players are currently in the doorway
    public bool IsPlayerInDoorway()
    {
        return playersInDoorway > 0;
    }
    
    // This method returns the number of players in the doorway
    public int GetPlayerCount()
    {
        return playersInDoorway;
    }
}