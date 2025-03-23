using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Shooter Settings")]
    [Tooltip("Projectile prefab to spawn.")]
    public GameObject projectilePrefab;

    [Tooltip("Interval between shots.")]
    public float shootInterval = 2f;

    [Tooltip("Speed at which the projectile travels.")]
    public float projectileSpeed = 5f;

    [Tooltip("Direction to shoot the projectile.")]
    public Vector2 shootDirection = Vector2.right;

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = shootDirection.normalized * projectileSpeed;
            }
        }
    }
}
