using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Tooltip("Time (in seconds) before this projectile self-destructs.")]
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var controller = other.GetComponent<Controller>();
        if (controller != null)
        {
            // Do something to the player (damage, knockback, etc.)
            // For now, just destroy the projectile
            Destroy(gameObject);
        }
    }
}
