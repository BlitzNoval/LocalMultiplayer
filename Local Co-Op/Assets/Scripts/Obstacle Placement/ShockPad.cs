using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ShockPad : MonoBehaviour
{
    [Header("Shock Settings")]
    [Tooltip("How long the player remains stunned.")]
    public float stunDuration = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller != null)
        {
            // Disable the player's movement scripts for a short time
            StartCoroutine(StunPlayer(other.gameObject));
        }
    }

    private IEnumerator StunPlayer(GameObject player)
    {
        Move move = player.GetComponent<Move>();
        Jump jump = player.GetComponent<Jump>();

        bool originalMoveState = false;
        bool originalJumpState = false;

        if (move != null)
        {
            originalMoveState = move.enabled;
            move.enabled = false;
        }
        if (jump != null)
        {
            originalJumpState = jump.enabled;
            jump.enabled = false;
        }

        yield return new WaitForSeconds(stunDuration);

        if (move != null)
            move.enabled = originalMoveState;
        if (jump != null)
            jump.enabled = originalJumpState;
    }
}
