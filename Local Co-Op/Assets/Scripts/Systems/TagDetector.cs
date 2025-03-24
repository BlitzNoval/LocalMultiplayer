

using UnityEngine;

public class PlayerTagger : MonoBehaviour
{
    [Header("Components")]
    public Collider2D tagTrigger; 
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    
    private PlayerTagState myTagState;
    
    void Start()
    {
        
        myTagState = GetComponent<PlayerTagState>();
        if (myTagState == null)
        {
            Debug.LogError("PlayerTagger: No PlayerTagState found on this object.");
        }
        
       
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
       
        if (!myTagState.isTagger) return;
        
 
        PlayerTagState otherState = other.GetComponent<PlayerTagState>();
        
        
        if (otherState == null)
        {
            otherState = other.GetComponentInParent<PlayerTagState>();
        }
        
        
        if (otherState == null || otherState == myTagState) return;
        
        // Skip if other player is already a tagger 
        if (otherState.isTagger || otherState.inGracePeriod) return;
        
        // We found a valid player to tag YES OMFG OMFG!
        if (showDebugLogs)
        {
            Debug.Log(gameObject.name + " tagged " + otherState.gameObject.name);
        }
        
        // Swap the roles
        TagManager.Instance.SwapRoles(myTagState, otherState);
    }
}