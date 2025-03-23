using UnityEngine;

public class DoorwayTrigger : MonoBehaviour
{
    // Flag to indicate if the player is in the doorway
    private bool isPlayerInside = false;

    // Called when another collider enters the trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter: " + other.name + " with tag: " + other.tag);
        if(other.CompareTag("Untagged"))
        {
            isPlayerInside = true;
            Debug.Log("Untagged detected in doorway: " + gameObject.name);
        }
    }

    // Called when another collider exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exit: " + other.name + " with tag: " + other.tag);
        if(other.CompareTag("Untagged"))
        {
            isPlayerInside = false;
            Debug.Log("Untagged left doorway: " + gameObject.name);
        }
    }

    // This method returns whether the player is currently in the doorway
    public bool IsPlayerInDoorway()
    {
        return isPlayerInside;
    }
}
