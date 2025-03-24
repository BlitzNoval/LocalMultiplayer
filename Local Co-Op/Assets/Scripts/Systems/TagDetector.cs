using UnityEngine;

public class PlayerTagger : MonoBehaviour
{
    [Header("Components")]
    public Collider2D tagTrigger; // Assign the trigger collider in inspector
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    
    private PlayerTagState myTagState;
    
    void Start()
    {
        // Get the player's tag state component
        myTagState = GetComponent<PlayerTagState>();
        if (myTagState == null)
        {
            Debug.LogError("PlayerTagger: No PlayerTagState found on this object.");
        }
        
        // Validate the tag trigger
        if (tagTrigger == null)
        {
            Debug.LogError("PlayerTagger: No tag trigger collider assigned.");
        }
        else if (!tagTrigger.isTrigger)
        {
            Debug.LogError("PlayerTagger: The assigned collider must be set as a trigger.");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only the tagger can tag others
        if (!myTagState.isTagger) return;
        
        // Find the other player's tag state
        PlayerTagState otherState = other.GetComponent<PlayerTagState>();
        
        // If it's not directly on this object, try to find it on the parent
        if (otherState == null)
        {
            otherState = other.GetComponentInParent<PlayerTagState>();
        }
        
        // Skip if no player found or if it's the same player
        if (otherState == null || otherState == myTagState) return;
        
        // Skip if other player is already a tagger or in grace period
        if (otherState.isTagger || otherState.inGracePeriod) return;
        
        // We found a valid player to tag!
        if (showDebugLogs)
        {
            Debug.Log(gameObject.name + " tagged " + otherState.gameObject.name);
        }
        
        // Swap the roles
        TagManager.Instance.SwapRoles(myTagState, otherState);
    }
}