using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FanBlower : MonoBehaviour
{
    [Header("Fan Settings")]
    [Tooltip("Direction of the wind force.")]
    public Vector2 blowDirection = new Vector2(-1, 0);

    [Tooltip("Strength of the wind force.")]
    public float blowForce = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller != null)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Apply a continuous force in the blow direction
                rb.AddForce(blowDirection.normalized * blowForce, ForceMode2D.Force);
            }
        }
    }
}
