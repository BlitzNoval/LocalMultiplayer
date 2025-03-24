using UnityEngine;

public class TagDetector : MonoBehaviour
{
    private PlayerTagState playerState;

    void Start()
    {
        playerState = GetComponentInParent<PlayerTagState>();
        if (playerState == null)
        {
            Debug.LogError("TagDetector: No PlayerTagState found on parent.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TagDetector"))
        {
            PlayerTagState otherState = other.GetComponentInParent<PlayerTagState>();
            if (otherState == null) return;

            // Ensure we don't tag a player who is still in grace period
            if (playerState.isTagger && !otherState.isTagger && !otherState.inGracePeriod)
            {
                TagManager.Instance.SwapRoles(playerState, otherState);
            }
        }
    }
}
