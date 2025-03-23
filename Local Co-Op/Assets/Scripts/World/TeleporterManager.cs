using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    public enum ChargeState { Off, Low, Medium, High, FullyCharged }

    [Header("Charge Settings")]
    // Test timer for demonstration (in seconds)
    private float timer = 0f;
    public float lowChargeTime = 20f;      // After 20 seconds: low charge
    public float mediumChargeTime = 40f;   // After 40 seconds: medium charge
    public float highChargeTime = 50f;     // After 50 seconds: high charge
    public float fullChargeTime = 60f;     // After 60 seconds: fully charged

    public ChargeState currentChargeState = ChargeState.Off;

    [Header("Visual Settings")]
    // Assign the SpriteRenderer on the teleporter tileset
    public SpriteRenderer spriteRenderer;
    // Sprites representing the various charge states (assign via Inspector)
    public Sprite offSprite;
    public Sprite lowChargeSprite;
    public Sprite mediumChargeSprite;
    public Sprite highChargeSprite;
    public Sprite fullChargeSprite;

    [Header("Teleport Settings")]
    // Destination for teleportation (assign via Inspector)
    public Transform teleportDestination;

    void Update()
    {
        // For testing: increment timer and update the teleporter charge state.
        timer += Time.deltaTime;
        UpdateChargeState();
    }

    /// <summary>
    /// Determines the current charge state based on the timer.
    /// </summary>
    void UpdateChargeState()
    {
        ChargeState newState = currentChargeState;
        if (timer < lowChargeTime)
        {
            newState = ChargeState.Off;
        }
        else if (timer < mediumChargeTime)
        {
            newState = ChargeState.Low;
        }
        else if (timer < highChargeTime)
        {
            newState = ChargeState.Medium;
        }
        else if (timer < fullChargeTime)
        {
            newState = ChargeState.High;
        }
        else
        {
            newState = ChargeState.FullyCharged;
        }

        if (newState != currentChargeState)
        {
            currentChargeState = newState;
            UpdateSprite();
        }
    }

    /// <summary>
    /// Updates the teleporter’s sprite based on its current charge state.
    /// </summary>
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            switch (currentChargeState)
            {
                case ChargeState.Off:
                    spriteRenderer.sprite = offSprite;
                    break;
                case ChargeState.Low:
                    spriteRenderer.sprite = lowChargeSprite;
                    break;
                case ChargeState.Medium:
                    spriteRenderer.sprite = mediumChargeSprite;
                    break;
                case ChargeState.High:
                    spriteRenderer.sprite = highChargeSprite;
                    break;
                case ChargeState.FullyCharged:
                    spriteRenderer.sprite = fullChargeSprite;
                    break;
            }
        }
    }

    /// <summary>
    /// When a player enters the teleporter trigger, if the teleporter is fully charged, teleport them instantly.
    /// The player's momentum (velocity) is preserved.
    /// </summary>
    /// <param name="other">Collider of the entering object</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Player" and the teleporter is fully charged.
        if (currentChargeState == ChargeState.FullyCharged && other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Vector2 preservedVelocity = Vector2.zero;
            if (rb != null)
            {
                preservedVelocity = rb.linearVelocity;
            }

            // Instantly move the player to the designated teleport destination.
            other.transform.position = teleportDestination.position;

            // Reapply the player's original velocity so momentum is maintained.
            if (rb != null)
            {
                rb.linearVelocity = preservedVelocity;
            }
        }
    }

    /// <summary>
    /// Resets the teleporter timer and state.
    /// This can be attached to a UI Button for repeated testing.
    /// </summary>
    public void ResetTeleporter()
    {
        timer = 0f;
        currentChargeState = ChargeState.Off;
        UpdateSprite();
    }
}
