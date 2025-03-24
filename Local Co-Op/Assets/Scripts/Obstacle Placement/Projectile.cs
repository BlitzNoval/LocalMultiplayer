using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Tooltip("Time (in seconds) before this projectile self-destructs.")]
    public float lifeTime = 5f;

    [Header("Player Effect Settings")]
    [Tooltip("Knockback force applied to the player on hit.")]
    public float knockbackForce = 5f;

    [Tooltip("Optional hit effect prefab to instantiate on impact.")]
    public GameObject hitEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
