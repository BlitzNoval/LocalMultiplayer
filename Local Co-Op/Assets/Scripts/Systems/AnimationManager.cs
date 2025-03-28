// Title: Animate like a programmer
// Author: TaroDev
// Date: 13 March  2025
// Availability: https://youtu.be/ZwLekxsSY3Y?si=_kpkE0Fupp2B1afD

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CollisionDataRetriever))]
public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator; 

    private Rigidbody2D body;
    private CollisionDataRetriever collisionDataRetriever;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        collisionDataRetriever = GetComponent<CollisionDataRetriever>();

        if (animator == null)
        {
            Debug.LogError("Animator is not assigned in AnimationManager!");
        }
    }

    private void Update()
    {
        if (animator == null) return;

        // Detect player states
        float horizontalSpeed = Mathf.Abs(body.linearVelocity.x);
        bool isInAir = !collisionDataRetriever.OnGround;
        bool isOnWall = collisionDataRetriever.OnWall && !collisionDataRetriever.OnGround;

        // Set animation parameters
        animator.SetFloat("Speed", horizontalSpeed);
        animator.SetBool("IsInAir", isInAir);
        animator.SetBool("IsOnWall", isOnWall);

        if (body.linearVelocity.x > 0.1f)
{
    animator.transform.localScale = new Vector3(1, 1, 1);
}
else if (body.linearVelocity.x < -0.1f)
{
    animator.transform.localScale = new Vector3(-1, 1, 1);
}
    }
}